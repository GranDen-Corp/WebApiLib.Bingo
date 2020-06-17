using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GranDen.Game.ApiLib.Bingo.Options
{
    public class BingoGameOption : List<BingoGameSetting>
    {
    }
    
    public class BingoGameSetting
    {
        [Required]
        public string GameName { get; set; }
        
        [Required]
        public string GameTableKey { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Width { get; set; }

        [Range(1, int.MaxValue)]
        public int Height { get; set; }
    }
}