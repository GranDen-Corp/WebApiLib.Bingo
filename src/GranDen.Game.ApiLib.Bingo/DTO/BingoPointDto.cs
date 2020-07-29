using System;
using GranDen.TimeLib.ClockShaft;

namespace GranDen.Game.ApiLib.Bingo.DTO
{
    /// <summary>
    /// <c>BingoPoint</c> DTO object
    /// </summary>
    public class BingoPointDto
    {
        /// <summary>
        /// This mark point X-axis value
        /// </summary>
        public int X { get; set; }
        
        /// <summary>
        /// This mark point Y-axis value
        /// </summary>
        public int Y { get; set; }
        
        /// <summary>
        /// The moment Mark point achieved 
        /// </summary>
        public DateTimeOffset ClearTime { get; set; } = ClockWork.DateTimeOffset.UtcNow;
    }
}
