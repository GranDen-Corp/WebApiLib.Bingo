using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;

namespace GranDen.Game.ApiLib.Bingo.Services
{
    /// <inheritdoc />
    public class PresetGeoPointService : IPresetGeoPointService
    {
        public IEnumerable<string> GeoPoints { get; set; }
    }
}
