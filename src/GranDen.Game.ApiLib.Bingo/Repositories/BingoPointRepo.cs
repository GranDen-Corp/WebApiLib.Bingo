using System;
using System.Collections.Generic;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.Exceptions;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GranDen.Game.ApiLib.Bingo.Repositories
{
    /// <inheritdoc />
    public class BingoPointRepo : IBingoPointRepo
    {
        private readonly BingoGameDbContext _bingoGameDbContext;
        private readonly IBingoGameInfoRepo _bingoGameInfoRepo;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="bingoGameDbContext"></param>
        /// <param name="bingoGameInfoRepo"></param>
        public BingoPointRepo(BingoGameDbContext bingoGameDbContext, IBingoGameInfoRepo bingoGameInfoRepo)
        {
            _bingoGameDbContext = bingoGameDbContext;
            _bingoGameInfoRepo = bingoGameInfoRepo;
        }


        /// <inheritdoc />
        public IEnumerable<BingoPoint> GetMappedBingoPoint(string bingoGameName, string bingoPlayerId, string geoPointId)
        {
            var geoPoint = _bingoGameDbContext.MappingGeoPoints.AsNoTracking()
                .Include(m => m.PointProjections).ThenInclude(p => p.BingoPlayerInfo)
                .Include(m => m.PointProjections).ThenInclude(p => p.BingoPoint).ThenInclude(b => b.BelongingGame)
                .Include(m => m.PointProjections).ThenInclude(p => p.BingoPoint).ThenInclude(b => b.BelongingPlayer)
                .FirstOrDefault(m =>
                    m.GeoPointId == geoPointId || m.GeoPointRedirected && m.RedirectedGeoPointId == geoPointId);

            if (geoPoint == null)
            {
                throw new Exception($"Geo Point Id '{geoPointId}' not exist.");
            }

            var pointProjections =
                geoPoint.PointProjections.Where(p => p.BingoPlayerInfo.PlayerId == bingoPlayerId).ToList();

            if (pointProjections.Count == 0)
            {
                throw new Exception($"Point Projection not exist.");
            }

            var ret = pointProjections.Where(p => p.BingoPoint.BelongingGame.GameName == bingoGameName)
                .Select(p => p.BingoPoint).ToList();

            if (ret.Count == 0)
            {
                throw new Exception(
                    $"Bingo mark point(s) at Geo Point Id '{geoPointId}' that belongs to player '{bingoPlayerId}' on Bingo Game '{bingoGameName}' not exist.");
            }

            return ret;
        }

        /// <inheritdoc />
        public IQueryable<BingoPoint> QueryBingoPoints(string bingoGameName, string bingoPlayerId)
        {
            var game = _bingoGameInfoRepo.QueryBingoGames().AsNoTracking().Include(g => g.JoinedPlayers)
                .FirstOrDefault(g =>
                    g.GameName == bingoGameName && g.JoinedPlayers.Any(p => p.PlayerId == bingoPlayerId));

            var player = _bingoGameDbContext.BingoPlayerInfos.AsNoTracking()
                .FirstOrDefault(p => p.PlayerId == bingoPlayerId);

            if (game == null || player == null)
            {
                throw new PlayerNotJoinedGameException(bingoGameName, bingoPlayerId);
            }

            return _bingoGameDbContext.BingoPoints.Where(p => p.BelongingGame == game && p.BelongingPlayer == player);
        }
    }
}
