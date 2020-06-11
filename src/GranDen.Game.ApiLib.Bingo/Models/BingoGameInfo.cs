using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    public class BingoGameInfo
    {
        [Key]
        public int Id { get; set; }

        public string GameName { get; set; }

        [Required]
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool Enabled { get; set; } = true;
        
        public IEnumerable<BingoPlayerInfo> JoinedPlayers { get; } = new List<BingoPlayerInfo>();

        public BingoGameInfo(string gameName, DateTimeOffset startTime, DateTimeOffset? endTime = null)
        {
            GameName = gameName;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}