using System;
using System.ComponentModel.DataAnnotations;
using GranDen.GameLib.Bingo;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    /// <summary>
    /// Bingo mark point entity
    /// </summary>
    public class BingoPoint
    {
        /// <summary>
        /// Bingo mark point identifier
        /// </summary>
        [Key]
        public int Id { get; set; }
       
        /// <summary>
        /// Mark point (x, y, true|false)
        /// </summary>
        public MarkPoint2D MarkPoint { get; set; }
       
        /// <summary>
        /// Bingo game entity of this bingo mark point
        /// </summary>
        public Bingo2dGameInfo BelongingGame { get; set; }

        /// <summary>
        /// Bingo game player entity of this bingo mark point
        /// </summary>
        public BingoPlayerInfo BelongingPlayer { get; set; }
        
        /// <summary>
        ///  GeoPoint ⟷ Bingo Mark Point conversion record
        /// </summary>
        public PointProjection PointProjection { get; set; }

        /// <summary>
        /// The moment Mark point achieved 
        /// </summary>
        public DateTimeOffset? ClearTime { get; set; }
    }
}
