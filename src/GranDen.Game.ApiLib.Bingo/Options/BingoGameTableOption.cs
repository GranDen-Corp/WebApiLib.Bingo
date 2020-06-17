using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GranDen.Game.ApiLib.Bingo.Options
{
    public class BingoGameTableOption : List<BingoGameTableSetting>
    {
    }

    public class BingoGameTableSetting
    {
       [Required] 
       public string GameTableKey { get; set; }
       
       [Required]
       public List<string> PrizeLines { get; set; }
    }
}
