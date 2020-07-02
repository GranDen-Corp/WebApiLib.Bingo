using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    /// <summary>
    /// Bingo Player Entity
    /// </summary>
    public class BingoPlayerInfo
    {
        /// <summary>
        /// Bingo Player Identifier
        /// </summary>
        [Key]
        [Column(TypeName = "varchar(128)")]
        public string PlayerId { get; set; }
        
        /// <summary>
        /// Joined Bingo Game(s) of this player
        /// </summary>
        public ICollection<Bingo2dGameInfo> JoinedGames { get; } = new List<Bingo2dGameInfo>();
    }
}
