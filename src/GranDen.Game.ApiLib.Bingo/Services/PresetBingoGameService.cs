using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;

namespace GranDen.Game.ApiLib.Bingo.Services
{
    public class PresetBingoGameService : IPresetBingoGameService
    {
        public IEnumerable<BingoGameInfoDto> GameInfoDtos { get; set; }
    }
}
