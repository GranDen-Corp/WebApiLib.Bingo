using System;
using System.ComponentModel.DataAnnotations;
using GranDen.GameLib.Bingo;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    public class BingoPoint
    {
        [Key]
        public int Id { get; set; }
       
        public MarkPoint2D MarkPoint { get; set; }
       
        public Bingo2dGameInfo BelongingGame { get; set; }

        public BingoPlayerInfo BelongingPlayer { get; set; }
        
        public PointProjection PointProjection { get; set; }

        public DateTimeOffset? ClearTime { get; set; }
    }
}