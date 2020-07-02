using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;

namespace GranDen.Game.ApiLib.Bingo.Services.Interfaces
{
    /// <summary>
    /// Geo Point assigner DI registration interface
    /// </summary>
    public interface IGeoPointIdProvider
    {
        /// <summary>
        /// Provide Geo Point assigner
        /// </summary>
        public GeoPointIdInitializeDelegate GeoPointIdInitializer { get; }
    }
}
