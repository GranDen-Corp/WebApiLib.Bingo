using System;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GranDen.Game.ApiLib.Bingo.Repositories
{
    public class BingoPointRepo : IBingoPointRepo
    {
        private readonly BingoGameDbContext _bingoGameDbContext;
        private readonly IBingoGameInfoRepo _bingoGameInfoRepo;

        public BingoPointRepo(BingoGameDbContext bingoGameDbContext, IBingoGameInfoRepo bingoGameInfoRepo)
        {
            _bingoGameDbContext = bingoGameDbContext;
            _bingoGameInfoRepo = bingoGameInfoRepo;
        }


        public BingoPoint GetMappedBingoPoint(string bingoGameName, string bingoPlayerId, string geoPointId)
        {
            var geoPoint = _bingoGameDbContext.MappingGeoPoints.AsNoTracking()
                .Include(m => m.PointProjections).ThenInclude(p => p.BingoPoint)
                .FirstOrDefault(m =>
                    m.GeoPointId == geoPointId || m.GeoPointRedirected && m.RedirectedGeoPointId == geoPointId);

            if (geoPoint == null)
            {
                throw new Exception($"Geo Point Id {geoPointId} not exist.");
            }

            var pointProjection =
                geoPoint.PointProjections.FirstOrDefault(p => p.BingoPlayerInfo.PlayerId == bingoPlayerId);

            if (pointProjection == null)
            {
                throw new Exception($"Point Projection not exist.");
            }

            return pointProjection.BingoPoint;
        }

        public IQueryable<BingoPoint> QueryBingoPoints(string bingoGameName, string bingoPlayerId)
        {
            var game = _bingoGameInfoRepo.QueryBingoGames().AsNoTracking().Include(g => g.JoinedPlayers)
                .FirstOrDefault(g =>
                    g.GameName == bingoGameName && g.JoinedPlayers.Any(p => p.PlayerId == bingoPlayerId));

            var player = _bingoGameDbContext.BingoPlayerInfos.AsNoTracking()
                .FirstOrDefault(p => p.PlayerId == bingoPlayerId);

            if (game == null || player == null)
            {
                throw new Exception($"Bingo Game {bingoGameName} not exist or player {bingoPlayerId} not joined.");
            }

            return _bingoGameDbContext.BingoPoints.Where(p => p.BelongingGame == game && p.BelongingPlayer == player);
        }
    }
}