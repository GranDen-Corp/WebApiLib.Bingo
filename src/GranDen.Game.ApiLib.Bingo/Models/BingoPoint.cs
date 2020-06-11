using System;
using System.ComponentModel.DataAnnotations;
using GranDen.GameLib.Bingo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    public class BingoPoint
    {
        [Key]
        public int Id { get; set; }
        
        public MarkPoint2D MarkPoint { get; set; }
        
        public PointProjection PointProjection { get; set; }

        public DateTimeOffset? ClearTime { get; set; }
    }

    public static class MarkPoint2DValueEfCoreUtil
    {
        public static ValueConverter<MarkPoint2D, string> GetMarkPoint2DValueConverter()
        {
            return new ValueConverter<MarkPoint2D, string>(
                toDb => $"{toDb.X}, {toDb.Y} | {toDb.Marked}",
                toEf => new MarkPoint2D(toEf)
                );
        }
    }
}