using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.Models;

namespace GranDen.Game.ApiLib.Bingo.Repositories.Interfaces
{
    /// <summary>
    /// <c>MappingGeoPoint</c> repository
    /// </summary>
    public interface IMappingGeoPointsRepo
    {
        /// <summary>
        /// Create a new <c>MappingGeoPoint</c> entity
        /// </summary>
        /// <param name="geoPointId"></param>
        /// <returns></returns>
        bool CreateMappingGeoPoint(string geoPointId);

        /// <summary>
        /// Create a lots of new <c>MappingGeoPoint</c> entities
        /// </summary>
        /// <param name="geoPointIds"></param>
        /// <returns></returns>
        bool CreateMappingGeoPoints(IEnumerable<string> geoPointIds);

        /// <summary>
        /// Update the redirection of a <c>MappingGeoPoint</c> entity
        /// </summary>
        /// <param name="geoPointId"></param>
        /// <param name="redirectGeoPointId"></param>
        /// <returns></returns>
        bool UpdateRedirection(string geoPointId, string redirectGeoPointId);

        /// <summary>
        /// Add <c>PointProjection</c> to a <c>MappingGeoPoint</c> entity
        /// </summary>
        /// <param name="geoPointId"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        bool AddProjection(string geoPointId, PointProjection projection);

        /// <summary>
        /// Get the exact Geo Point <c>MappingGeoPoint</c> entity that belong to a given Bingo Game &amp; player Id &amp; (x, y) bingo table coordinate
        /// </summary>
        /// <param name="bingoGameName"></param>
        /// <param name="bingoPlayerId"></param>
        /// <param name="bingoPointCoordinate"></param>
        /// <returns></returns>
        MappingGeoPoint GetPlayerGeoPoint(string bingoGameName, string bingoPlayerId, (int x, int y) bingoPointCoordinate);
    }
}
