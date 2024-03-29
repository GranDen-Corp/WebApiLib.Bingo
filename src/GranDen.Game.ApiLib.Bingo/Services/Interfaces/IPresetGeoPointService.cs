﻿using System.Collections.Generic;

namespace GranDen.Game.ApiLib.Bingo.Services.Interfaces
{
    /// <summary>
    /// Provide preset <c>MappingGeoPoint</c> data
    /// </summary>
    public interface IPresetGeoPointService
    {
        /// <summary>
        /// Preset geo point id(s)
        /// </summary>
        public IEnumerable<string> GeoPoints { get; set; }
    }
}
