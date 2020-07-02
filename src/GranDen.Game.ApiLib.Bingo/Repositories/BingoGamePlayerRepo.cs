using System;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.GameLib.Bingo;
using Microsoft.EntityFrameworkCore;

namespace GranDen.Game.ApiLib.Bingo.Repositories
{
    /// <inheritdoc />
    public class BingoGamePlayerRepo : IBingoGamePlayerRepo
    {
        private readonly BingoGameDbContext _bingoGameDbContext;
        private readonly IBingoGameInfoRepo _bingoGameInfoRepo;
        private readonly IBingoPointRepo _bingoPointRepo;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="bingoGameDbContext"></param>
        /// <param name="bingoGameInfoRepo"></param>
        /// <param name="bingoPointRepo"></param>
        public BingoGamePlayerRepo(BingoGameDbContext bingoGameDbContext, IBingoGameInfoRepo bingoGameInfoRepo,
            IBingoPointRepo bingoPointRepo)
        {
            _bingoGameDbContext = bingoGameDbContext;
            _bingoGameInfoRepo = bingoGameInfoRepo;
            _bingoPointRepo = bingoPointRepo;
        }

        /// <inheritdoc />
        public IQueryable<BingoPlayerInfo> GetBingoGamePlayer(string bingoPlayerId)
        {
            return _bingoGameDbContext.BingoPlayerInfos.Where(p => p.PlayerId == bingoPlayerId);
        }

        /// <inheritdoc />
        public IQueryable<BingoPlayerInfo> QueryBingoPlayer()
        {
            return _bingoGameDbContext.BingoPlayerInfos.AsQueryable();
        }

        /// <inheritdoc />
        public BingoPlayerInfo InitBingoGamePlayerData(string bingoGameName, string bingoPlayerId,
            GeoPointIdInitializeDelegate geoPointIdInitiator)
        {
            var bingoGame = _bingoGameInfoRepo.GetByName(bingoGameName);

            var bingoPlayer = new BingoPlayerInfo {PlayerId = bingoPlayerId};
            bingoPlayer.JoinedGames.Add(bingoGame);

            var geoPointIds = geoPointIdInitiator(bingoGameName, bingoPlayerId);

            for (var x = 0; x < bingoGame.MaxWidth; x++)
            {
                for (var y = 0; y < bingoGame.MaxHeight; y++)
                {
                    var bingoPoint = new BingoPoint
                    {
                        MarkPoint = new MarkPoint2D(x, y, false),
                        BelongingGame = bingoGame,
                        BelongingPlayer = bingoPlayer
                    };

                    var geoPointId = geoPointIds[(x, y)];
                    var mappingGeoPoint =
                        _bingoGameDbContext.MappingGeoPoints.FirstOrDefault(m => m.GeoPointId == geoPointId);

                    if (mappingGeoPoint == null)
                    {
                        throw new Exception($"GeoPoint {geoPointId} not exist.");
                    }

                    var pointProjection = new PointProjection
                    {
                        BingoPlayerInfo = bingoPlayer,
                        BingoPoint = bingoPoint
                    };
                    mappingGeoPoint.PointProjections.Add(pointProjection);
                    pointProjection.MappingGeoPoint = mappingGeoPoint;
                    bingoPoint.PointProjection = pointProjection;

                    _bingoGameDbContext.BingoPoints.Add(bingoPoint);
                }
            }

            bingoGame.JoinedPlayers.Add(bingoPlayer);

            var updateCount = _bingoGameDbContext.SaveChanges();
            if (updateCount < 1)
            {
                throw new Exception($"Save {bingoPlayerId} error!");
            }

            return bingoPlayer;
        }

        /// <inheritdoc />
        public bool ResetBingoGamePlayerData(string bingoGameName, string bingoPlayerId)
        {
            var bingoGame = _bingoGameInfoRepo.GetByName(bingoGameName);

            if (bingoGame == null)
            {
                throw new Exception($"Game {bingoGameName} not exist.");
            }

            var player = _bingoGameDbContext.BingoPlayerInfos.Include(p => p.JoinedGames)
                .FirstOrDefault(p =>
                    p.PlayerId == bingoPlayerId && p.JoinedGames.Any(g => g.GameName == bingoGameName));

            if (player == null)
            {
                throw new Exception($"Player {bingoPlayerId} not exist or not joined in Game {bingoGameName}.");
            }

            //NOTE: Should it be able to safely do automatically cascading delete in future version of EF Core?
            var bingoPoints =
                _bingoPointRepo.QueryBingoPoints(bingoGameName, bingoPlayerId).Include(p => p.PointProjection)
                    .ThenInclude(p => p.MappingGeoPoint);

            foreach (var bingoPoint in bingoPoints)
            {
                var pointProjection = bingoPoint.PointProjection;
                pointProjection.MappingGeoPoint.PointProjections.Remove(pointProjection);
                _bingoGameDbContext.Remove(pointProjection);
                _bingoGameDbContext.Remove(bingoPoint);
                bingoGame.BingoPoints.Remove(bingoPoint);
            }

            bingoGame.JoinedPlayers.Remove(player);

            var updateCount = _bingoGameDbContext.SaveChanges();

            return updateCount > 0;
        }
    }
}
