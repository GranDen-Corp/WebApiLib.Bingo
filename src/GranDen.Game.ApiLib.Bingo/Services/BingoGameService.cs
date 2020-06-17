using System;
using System.Collections.Generic;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Options;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.GameLib.Bingo;
using Microsoft.Extensions.Options;

namespace GranDen.Game.ApiLib.Bingo.Services
{
    public class BingoGameService : IBingoGameService<string>
    {
        private readonly BingoGameDbContext _bingoGameDbContext;
        private readonly IBingoGameInfoRepo _bingoGameInfoRepo;
        private readonly IBingoGamePlayerRepo _bingoGamePlayerRepo;
        private readonly IBingoPointRepo _bingoPointRepo;
        private readonly IOptionsMonitor<BingoGameOption> _bingoGameOptionDelegate;
        private readonly IOptionsMonitor<BingoGameTableOption> _bingoGameTableOptionDelegate;

        public BingoGameService(BingoGameDbContext bingoGameDbContext,
            IBingoGameInfoRepo bingoGameInfoRepo, IBingoGamePlayerRepo bingoGamePlayerRepo, IBingoPointRepo bingoPointRepo, 
            IOptionsMonitor<BingoGameOption> bingoGameOptionDelegate, IOptionsMonitor<BingoGameTableOption> bingoGameTableOptionDelegate)
        {
            _bingoGameDbContext = bingoGameDbContext;
            _bingoGameInfoRepo = bingoGameInfoRepo;
            _bingoGamePlayerRepo = bingoGamePlayerRepo;
            _bingoPointRepo = bingoPointRepo;
            _bingoGameOptionDelegate = bingoGameOptionDelegate;
            _bingoGameTableOptionDelegate = bingoGameTableOptionDelegate;
        }

        public ICollection<BingoGameInfoDto> GetAttendableGames(DateTimeOffset current)
        {
           var games =  
               _bingoGameDbContext.Bingo2dGameInfos
                   .Where( x => x.Enabled && x.StartTime <= current && (!x.EndTime.HasValue || current < x.EndTime))
                   .Select(g => new BingoGameInfoDto
                       {GameName = g.GameName, Enabled = g.Enabled, StartTime = g.StartTime, EndTime = g.EndTime}).ToList();

           return games;
        }

        public bool JoinGame(string gameName, string playerId)
        {
            var game = _bingoGameInfoRepo.GetByName(gameName);
            
            if (game == null)
            {
                var bingoGameInfoDto = new BingoGameInfoDto
                {
                    GameName = gameName,
                    Enabled = true,
                    StartTime = DateTimeOffset.UtcNow
                };
                _bingoGameInfoRepo.CreateBingoGame(bingoGameInfoDto);

                game = _bingoGameInfoRepo.GetByName(gameName);
            }

            if (game.JoinedPlayers.Any(p => p.PlayerId == playerId))
            {
                return false;
            }

            var player = _bingoGamePlayerRepo.GetBingoGamePlayer(playerId).FirstOrDefault();
            if (player == null)
            {
                player = new BingoPlayerInfo{ PlayerId = playerId};
                _bingoGameDbContext.BingoPlayerInfos.Add(player);
            }
            player.JoinedGames.Add(game);
            game.JoinedPlayers.Add(player);

            var updateCount = _bingoGameDbContext.SaveChanges();

            return updateCount > 0;
        }

        public bool MarkBingoPoint(string gameName, string playerId, (int x, int y) point)
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
            bingoPoint.ClearTime = DateTimeOffset.UtcNow;
            var updateCount = _bingoGameDbContext.SaveChanges();

            return updateCount > 0;
        }

        public ICollection<MarkPoint2D> GetPlayerBingoPointStatus(string gameName, string playerId)
        {
            return _bingoPointRepo.QueryBingoPoints(gameName, playerId).Select(b => b.MarkPoint).ToList();
        }

        public ICollection<string> GetAchievedBingoPrizes(string gameName, string playerId)
        {
            var bingoGameSetting = _bingoGameOptionDelegate.CurrentValue.FirstOrDefault(g => g.GameName == gameName);
            if (bingoGameSetting == null)
            {
                throw new Exception($"Bingo game {gameName} setting not found.");
            }

            var gameTableSetting =
                _bingoGameTableOptionDelegate.CurrentValue.FirstOrDefault(t =>
                    t.GameTableKey == bingoGameSetting.GameTableKey);

            if (gameTableSetting == null || !gameTableSetting.PrizeLines.Any())
            {
                throw new Exception($"Bingo game table {bingoGameSetting.GameTableKey} not found or no prize settings exist.");
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