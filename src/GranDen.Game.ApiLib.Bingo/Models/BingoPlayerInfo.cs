using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    public class BingoPlayerInfo
    {
        [Key]
        [Column(TypeName = "varchar(128)")]
        public string PlayerId { get; set; }
        
        public BingoGameInfo BingoGame { get; set; }
        
        public ICollection<BingoPoint> BingoPoints { get; } = new List<BingoPoint>();
        
    }
}