using System;

namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Throws when specified not exist geo point
    /// </summary>
    public class GeoPointNotExistException : Exception
    {
        /// <summary>
        /// Geo Point Id
        /// </summary>
        public string GeoPointId { get; }
        
        /// <summary>
        /// Exception class constructor
        /// </summary>
        /// <param name="geoPointId"></param>
        public GeoPointNotExistException(string geoPointId) : base($"GeoPoint {geoPointId} not exist.")
        {
            GeoPointId = geoPointId;
        } 
    }
}
