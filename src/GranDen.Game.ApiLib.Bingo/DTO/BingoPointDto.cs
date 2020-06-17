using System;

namespace GranDen.Game.ApiLib.Bingo.DTO
{
    public class BingoPointDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string GeoPointId { get; set; }
        public DateTimeOffset? ClearTime { get; set; }
    }
}