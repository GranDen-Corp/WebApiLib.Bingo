using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    /// <summary>
    /// Bingo Game entity
    /// </summary>
    public class Bingo2dGameInfo
    {
        /// <summary>
        /// Entity Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Bingo Game Identifier
        /// </summary>
        [Required]
        public string GameName { get; set; }

        /// <summary>
        /// Bingo Game Internationalization display key
        /// </summary>
        public string I18nDisplayKey { get; set; }

        /// <summary>
        ///  X-axis range of bingo game 
        /// </summary>
        public int MaxWidth { get; }
        
        /// <summary>
        /// Y-axis range of bingo game
        /// </summary>
        public int MaxHeight { get; }

        /// <summary>
        /// When the bingo game can start to let player collect bingo mark point
        /// </summary>
        [Required]
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// The time when the bingo game stop and player cannot collect more bingo mark point
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// Game status flag, when disabled, player cannot join or collect bingo point of the game
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Joined players of ths bingo game
        /// </summary>
        public ICollection<BingoPlayerInfo> JoinedPlayers { get; } = new List<BingoPlayerInfo>();
        
        /// <summary>
        /// Bingo mark points entities belong to this bingo game
        /// </summary>
        public ICollection<BingoPoint> BingoPoints { get; } = new List<BingoPoint>();

        /// <summary>
        /// Class constructor 
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
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
