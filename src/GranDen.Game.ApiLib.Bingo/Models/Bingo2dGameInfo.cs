using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    public class Bingo2dGameInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string GameName { get; set; }

        public string I18nDisplayKey { get; set; }

        public int MaxWidth { get; }
        public int MaxHeight { get; }

        [Required]
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool Enabled { get; set; } = true;

        public ICollection<BingoPlayerInfo> JoinedPlayers { get; } = new List<BingoPlayerInfo>();
        
        public ICollection<BingoPoint> BingoPoints { get; } = new List<BingoPoint>();

        public Bingo2dGameInfo(string gameName, int maxWidth, int maxHeight, DateTimeOffset startTime, DateTimeOffset? endTime = null)
        {
            GameName = gameName;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}