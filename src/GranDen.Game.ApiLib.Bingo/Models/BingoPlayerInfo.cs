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
        
        public ICollection<Bingo2dGameInfo> JoinedGames { get; set; }
    }
}