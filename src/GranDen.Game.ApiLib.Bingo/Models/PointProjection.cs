using System.ComponentModel.DataAnnotations;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    /// <summary>
    /// MarkPoint ⟷ GeoPoint conversion entity
    /// </summary>
    public class PointProjection
    {
        /// <summary>
        /// Entity Id
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// Belonged Bingo game player
        /// </summary>
        public BingoPlayerInfo BingoPlayerInfo { get; set; }
        
        /// <summary>
        /// Bingo game marked point
        /// </summary>
        public BingoPoint BingoPoint { get; set; }
        
        /// <summary>
        /// Foreign Key of Bingo game marked point, automatically accessed by EF Core
        /// </summary>
        public int BingoPointFk { get; set; }

        /// <summary>
        /// GeoPoint mapping entity
        /// </summary>
        public MappingGeoPoint MappingGeoPoint { get; set; }
    }
}
