using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.Models;

namespace GranDen.Game.ApiLib.Bingo.Repositories.Interfaces
{
    public interface IMappingGeoPointsRepo
    {
        bool CreateMappingGeoPoint(string geoPointId);

        bool CreateMappingGeoPoints(IEnumerable<string> geoPointIds);

        bool UpdateRedirection(string geoPointId, string redirectGeoPointId);
        
        bool AddProjection(string geoPointId, PointProjection projection);
        
        MappingGeoPoint GetPlayerGeoPoint(string bingoGameName, string bingoPlayerId, (int x, int y) bingoPointCoordinate);
    }
}