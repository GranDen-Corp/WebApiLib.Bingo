using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;

namespace GranDen.Game.ApiLib.Bingo.Services
{
    /// <inheritdoc />
    public class PresetGeoPointService : IPresetGeoPointService
    {
        /// <inheritdoc />
        public IEnumerable<string> GeoPoints { get; set; }
    }
}
