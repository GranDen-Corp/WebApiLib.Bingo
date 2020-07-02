using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;

namespace GranDen.Game.ApiLib.Bingo.Services
{
    /// <inheritdoc />
    public class GeoPointIdProvider : IGeoPointIdProvider
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="geoPointIdInitializeDelegate"></param>
        public GeoPointIdProvider(GeoPointIdInitializeDelegate geoPointIdInitializeDelegate)
        {
            GeoPointIdInitializer = geoPointIdInitializeDelegate;
        }

        /// <inheritdoc />
        public GeoPointIdInitializeDelegate GeoPointIdInitializer { get;}
    }
}
