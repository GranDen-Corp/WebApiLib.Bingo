using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;

namespace GranDen.Game.ApiLib.Bingo.Services.Interfaces
{
    public interface IGeoPointIdProvider
    {
        public GeoPointIdInitializeDelegate GeoPointIdInitializer { get; }
    }
}