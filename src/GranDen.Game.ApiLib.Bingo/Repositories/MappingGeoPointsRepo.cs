using System;
using System.Collections.Generic;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GranDen.Game.ApiLib.Bingo.Repositories
{
    public class MappingGeoPointsRepo : IMappingGeoPointsRepo
    {
        private readonly BingoGameDbContext _bingoGameDbContext;
        private readonly IBingoPointRepo _bingoPointRepo;

        public MappingGeoPointsRepo(BingoGameDbContext bingoGameDbContext,
            IBingoPointRepo bingoPointRepo)
        {
            _bingoGameDbContext = bingoGameDbContext;
            _bingoPointRepo = bingoPointRepo;
        }

        public bool CreateMappingGeoPoint(string geoPointId)
        {
            var mappingGeoPoint = _bingoGameDbContext.MappingGeoPoints.FirstOrDefault(p => p.GeoPointId == geoPointId);

            if (mappingGeoPoint != null)
            {
                throw new Exception($"GeoPoint {geoPointId} already existed.");
            }

            mappingGeoPoint = new MappingGeoPoint {GeoPointId = geoPointId};
            _bingoGameDbContext.MappingGeoPoints.Add(mappingGeoPoint);
            var updateCount = _bingoGameDbContext.SaveChanges();

            return updateCount >= 1;
        }

        public bool CreateMappingGeoPoints(IEnumerable<string> geoPointIds)
        {
            foreach (var geoPointId in geoPointIds)
            {
                var mappingGeoPoint =
                    _bingoGameDbContext.MappingGeoPoints.FirstOrDefault(p => p.GeoPointId == geoPointId);

                if (mappingGeoPoint != null)
                {
                    continue;
                }

                mappingGeoPoint = new MappingGeoPoint {GeoPointId = geoPointId};
                _bingoGameDbContext.MappingGeoPoints.Add(mappingGeoPoint);
            }

            _bingoGameDbContext.SaveChanges();
            return true;
        }


        public bool UpdateRedirection(string geoPointId, string redirectGeoPointId)
        {
            var mappingGeoPoint = _bingoGameDbContext.MappingGeoPoints.FirstOrDefault(p => p.GeoPointId == geoPointId);

            if (mappingGeoPoint == null)
            {
                throw new Exception($"GeoPoint {geoPointId} not exist.");
            }

            mappingGeoPoint.GeoPointRedirected = true;
            mappingGeoPoint.RedirectedGeoPointId = redirectGeoPointId;

            var updateCount = _bingoGameDbContext.SaveChanges();

            return updateCount >= 1;
        }

        public bool AddProjection(string geoPointId, PointProjection projection)
        {
            var mappingGeoPoint = _bingoGameDbContext.MappingGeoPoints.Include(m => m.PointProjections)
                .FirstOrDefault(p => p.GeoPointId == geoPointId);

            if (mappingGeoPoint == null)
            {
                throw new Exception($"GeoPoint {geoPointId} not exist.");
            }

            mappingGeoPoint.PointProjections.Add(projection);

            var updateCount = _bingoGameDbContext.SaveChanges();

            return updateCount >= 1;
        }

        public MappingGeoPoint GetPlayerGeoPoint(string bingoGameName, string bingoPlayerId,
            (int x, int y) bingoPointCoordinate)
        {
            var (x, y) = bingoPointCoordinate;
            var bingoPoint = _bingoPointRepo.QueryBingoPoints(bingoGameName, bingoPlayerId)
                .Include(b => b.PointProjection).ThenInclude(p => p.MappingGeoPoint)
                .First(p => p.MarkPoint.X == x && p.MarkPoint.Y == y);

            if (bingoPoint == null)
            {
                throw new Exception(
                    $"BingoPoint ({x},{y}) of Player {bingoPlayerId} in BingoGame {bingoGameName} not exist.");
            }

            return bingoPoint.PointProjection.MappingGeoPoint;
        }
    }
}