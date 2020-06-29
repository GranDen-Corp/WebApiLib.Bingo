using System.ComponentModel.DataAnnotations;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    public class PointProjection
    {
        [Key]
        public int Id { get; set; }
        
        public BingoPlayerInfo BingoPlayerInfo { get; set; }
        
        public BingoPoint BingoPoint { get; set; }
        
        public int BingoPointFk { get; set; }

        public MappingGeoPoint MappingGeoPoint { get; set; }
    }
}