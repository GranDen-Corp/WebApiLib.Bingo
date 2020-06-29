using System;

namespace GranDen.Game.ApiLib.Bingo.DTO
{
    public class BingoGameInfoDto
    {
        public string GameName { get; set; }
        
        public string I18nDisplayKey { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool Enabled { get; set; } = true;
    }
}