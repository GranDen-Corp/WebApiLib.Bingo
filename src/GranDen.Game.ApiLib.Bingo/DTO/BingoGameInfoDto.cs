using System;

namespace GranDen.Game.ApiLib.Bingo.DTO
{
    /// <summary>
    /// <c>Bingo2dGameInfo</c> DTO object
    /// </summary>
    public class BingoGameInfoDto
    {
        /// <summary>
        /// Bingo Game Identifier
        /// </summary>
        public string GameName { get; set; }

        /// <summary>
        /// Bingo Game Internationalization display key
        /// </summary>
        public string I18nDisplayKey { get; set; }

        /// <summary>
        /// When the bingo game can start to let player collect bingo mark point
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// The time when the bingo game stop and player cannot collect more bingo mark point
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// Game status flag, when disabled, player cannot join or collect bingo point of the game
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}
