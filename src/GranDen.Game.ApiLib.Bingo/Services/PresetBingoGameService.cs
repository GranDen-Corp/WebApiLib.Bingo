using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;

namespace GranDen.Game.ApiLib.Bingo.Services
{
    /// <inheritdoc />
    public class PresetBingoGameService : IPresetBingoGameService
    {
        /// <inheritdoc />
        public IEnumerable<BingoGameInfoDto> GameInfoDtos { get; set; }
    }
}
