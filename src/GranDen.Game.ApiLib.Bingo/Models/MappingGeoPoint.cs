using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    public class MappingGeoPoint
    {
        [Key]
        public int Id { get; set; }
       
        [Required]
        [Column(TypeName = "varchar(256)")]
        public string GeoPointId { get; set; }

        public  ICollection<PointProjection> PointProjections { get; } = new List<PointProjection>();

        //set to true if for some reason, the original GeoPoint abandoned then the new GeoPointId must be resolved. 
        public bool GeoPointRedirected { get; set; } = false;

        public string RedirectedGeoPointId { get; set; }
    }
}