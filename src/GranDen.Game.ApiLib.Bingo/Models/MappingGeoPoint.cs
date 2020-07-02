using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    /// <summary>
    /// Geo Point entity
    /// </summary>
    public class MappingGeoPoint
    {
        /// <summary>
        /// Entity identity that is manipulated by EF Core
        /// </summary>
        [Key]
        public int Id { get; set; }
       
        /// <summary>
        /// Geo Point ID that is point to real geo point data in another real Geographic system 
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(256)")]
        public string GeoPointId { get; set; }

        /// <summary>
        /// store the MarkPoint ⟷ GeoPoint conversion entities
        /// </summary>
        public  ICollection<PointProjection> PointProjections { get; } = new List<PointProjection>();

        /// <summary>
        /// Set to true if for some situation, the original GeoPoint abandoned then another <c>MappingGeoPoint</c> must be resolved. 
        /// </summary>
        public bool GeoPointRedirected { get; set; } = false;

        /// <summary>
        /// The pointed another GeoPointId value of <c>MappingGeoPoint</c>
        /// </summary>
        public string RedirectedGeoPointId { get; set; }
    }
}
