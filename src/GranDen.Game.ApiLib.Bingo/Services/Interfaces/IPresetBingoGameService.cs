using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.DTO;

namespace GranDen.Game.ApiLib.Bingo.Services.Interfaces
{
    /// <summary>
    /// Provide preset <c>BingoGameInfoDto</c> data
    /// </summary>
    public interface IPresetBingoGameService
    {
        /// <summary>
        /// Preset Bingo Game data
        /// </summary>
        IEnumerable<BingoGameInfoDto> GameInfoDtos { get; set; }
    }
}
