using System;
using System.Collections.Generic;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Exceptions;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Options;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;
using GranDen.GameLib.Bingo;
using GranDen.TimeLib.ClockShaft;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GranDen.Game.ApiLib.Bingo.Services
{
    /// <inheritdoc />
    public class BingoGameService : IBingoGameService<string>
    {
        private readonly BingoGameDbContext _bingoGameDbContext;
        private readonly IBingoGameInfoRepo _bingoGameInfoRepo;
        private readonly IBingoGamePlayerRepo _bingoGamePlayerRepo;
        private readonly IBingoPointRepo _bingoPointRepo;
        private readonly IGeoPointIdProvider _geoPointIdProvider;
        private readonly IOptionsMonitor<BingoGameOption> _bingoGameOptionDelegate;
        private readonly IOptionsMonitor<BingoGameTableOption> _bingoGameTableOptionDelegate;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="bingoGameDbContext"></param>
        /// <param name="bingoGameInfoRepo"></param>
        /// <param name="bingoGamePlayerRepo"></param>
        /// <param name="bingoPointRepo"></param>
        /// <param name="geoPointIdProvider"></param>
        /// <param name="bingoGameOptionDelegate"></param>
        /// <param name="bingoGameTableOptionDelegate"></param>
        public BingoGameService(BingoGameDbContext bingoGameDbContext,
            IBingoGameInfoRepo bingoGameInfoRepo, IBingoGamePlayerRepo bingoGamePlayerRepo, IBingoPointRepo bingoPointRepo,
            IGeoPointIdProvider geoPointIdProvider,
            IOptionsMonitor<BingoGameOption> bingoGameOptionDelegate,
            IOptionsMonitor<BingoGameTableOption> bingoGameTableOptionDelegate)
        {
            _bingoGameDbContext = bingoGameDbContext;
            _bingoGameInfoRepo = bingoGameInfoRepo;
            _bingoGamePlayerRepo = bingoGamePlayerRepo;
            _bingoPointRepo = bingoPointRepo;
            _geoPointIdProvider = geoPointIdProvider;
            _bingoGameOptionDelegate = bingoGameOptionDelegate;
            _bingoGameTableOptionDelegate = bingoGameTableOptionDelegate;
        }

        /// <inheritdoc />
        public ICollection<BingoGameInfoDto> GetAttendableGames(DateTimeOffset current)
        {
            var games =
                _bingoGameDbContext.Bingo2dGameInfos
                    .Where(x => x.Enabled && x.StartTime <= current && (!x.EndTime.HasValue || current < x.EndTime))
                    .Select(g => new BingoGameInfoDto
                    {
                        GameName = g.GameName, Enabled = g.Enabled, StartTime = g.StartTime, EndTime = g.EndTime
                    }).ToList();

            return games;
        }

        /// <inheritdoc />
        public bool JoinGame(string gameName, string playerId)
        {
            var game = _bingoGameInfoRepo.GetByName(gameName);

            if (game == null)
            {
                throw new GameNotExistException(gameName);
            }

            if (!game.Enabled)
            {
                throw new GameDisabledException(gameName);
            }

            if (game.EndTime.HasValue && game.EndTime <= ClockWork.DateTimeOffset.UtcNow)
            {
                throw new GameExpiredException(gameName);
            }

            if (game.JoinedPlayers.Any(p => p.PlayerId == playerId))
            {
                return false;
            }

            var player = _bingoGamePlayerRepo.GetBingoGamePlayer(playerId).FirstOrDefault() ??
                         _bingoGamePlayerRepo.InitBingoGamePlayerData(gameName, playerId,
                             _geoPointIdProvider.GeoPointIdInitializer);

            return player.JoinedGames.Any(g => g.GameName == gameName);
        }

        /// <inheritdoc />
        public bool MarkBingoPoint(string gameName, string playerId, (int x, int y) point, DateTimeOffset markedTime)
        {
            var (x, y) = point;
            var bingoPoint = _bingoPointRepo.QueryBingoPoints(gameName, playerId)
                .FirstOrDefault(p => p.MarkPoint.X == x && p.MarkPoint.Y == y);

            if (bingoPoint == null)
            {
                throw new Exception($"No such bingo point ({x}, {y}) for Player {playerId} in Game {gameName}.");
            }

            if (bingoPoint.MarkPoint.Marked)
            {
                return true;
            }

            bingoPoint.MarkPoint.Marked = true;
            bingoPoint.ClearTime = markedTime;
            var updateCount = _bingoGameDbContext.SaveChanges();

            return updateCount > 0;
        }

        /// <inheritdoc />
        public bool MarkBingoPoint(string gameName, string playerId, BingoPointDto bingoPointDto)
        {
            return MarkBingoPoint(gameName, playerId, (bingoPointDto.X, bingoPointDto.Y), bingoPointDto.ClearTime);
        }

        /// <inheritdoc />
        public ICollection<MarkPoint2D> GetPlayerBingoPointStatus(string gameName, string playerId)
        {
            return _bingoPointRepo.QueryBingoPoints(gameName, playerId).AsNoTracking().Select(b => b.MarkPoint).ToList();
        }

        /// <inheritdoc />
        public ICollection<string> GetAchievedBingoPrizes(string gameName, string playerId)
        {
            var game = _bingoGameInfoRepo.GetByName(gameName);

            if (game == null)
            {
                throw new GameNotExistException(gameName);
            }

            var bingoGameSetting = _bingoGameOptionDelegate.CurrentValue.FirstOrDefault(g => g.GameName == gameName);
            if (bingoGameSetting == null)
            {
                throw new GameSettingNotFoundException(gameName);
            }

            var gameTableSetting =
                _bingoGameTableOptionDelegate.CurrentValue.FirstOrDefault(t =>
                    t.GameTableKey == bingoGameSetting.GameTableKey);

            if (gameTableSetting == null)
            {
                throw new GameTableSettingNotfoundException(gameName, bingoGameSetting.GameTableKey);
            }

            var bingoPoints = _bingoPointRepo.QueryBingoPoints(gameName, playerId).ToList();
            if (!bingoPoints.Any())
            {
                return new List<string>();
            }

            var prizeLines = gameTableSetting.PrizeLines.Select(s => s.ToPrizeLine2d()).ToList();
            var bingo = new Bingo2dPrizeClincher(prizeLines);

            var markPoints = bingoPoints.Select(b => b.MarkPoint).ToList();
            return bingo.Decide(markPoints);
        }
    }
}
