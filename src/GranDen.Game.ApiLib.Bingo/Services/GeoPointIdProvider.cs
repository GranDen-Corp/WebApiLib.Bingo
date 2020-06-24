using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;

namespace GranDen.Game.ApiLib.Bingo.Services
{
    public class GeoPointIdProvider : IGeoPointIdProvider
    {
        public GeoPointIdProvider(GeoPointIdInitializeDelegate geoPointIdInitializeDelegate)
        {
            GeoPointIdInitializer = geoPointIdInitializeDelegate;
        }
        
        public GeoPointIdInitializeDelegate GeoPointIdInitializer { get;}
    }
}