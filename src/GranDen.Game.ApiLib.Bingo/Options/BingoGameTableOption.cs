using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GranDen.Game.ApiLib.Bingo.Options
{
    /// <summary>
    /// Bingo Game Table typed option
    /// </summary>
    public class BingoGameTableOption : List<BingoGameTableSetting>
    {
    }

    /// <summary>
    /// Bingo Game Table setting definition
    /// </summary>
    public class BingoGameTableSetting
    {
        /// <summary>
        /// Game Table identifier
        /// </summary>
        [Required]
        public string GameTableKey { get; set; }

        /// <summary>
        /// Prize line entries 
        /// </summary>
        /// <value>Must be (x1, y1), (x2, y2), (x3, y3), ... | 'Prize Name' format string list</value>
        [Required]
        public List<string> PrizeLines { get; set; }
    }
}
