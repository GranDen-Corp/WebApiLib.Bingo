using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GranDen.Game.ApiLib.Bingo.Options
{
    /// <summary>
    /// Bingo Game typed option
    /// </summary>
    public class BingoGameOption : List<BingoGameSetting>
    {
    }

    /// <summary>
    /// Single Bingo Game setting definition
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BingoGameSetting
    {
        /// <summary>
        /// Bingo Game display Name
        /// </summary>
        [Required]
        public string GameName { get; set; }
        
        /// <summary>
        /// Language Display Id
        /// </summary>
        public string I18nKey { get; set; }

        /// <summary>
        /// Bingo Game Table key
        /// </summary>
        [Required]
        public string GameTableKey { get; set; }

        /// <summary>
        ///  X-axis range of bingo game 
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Width { get; set; }

        /// <summary>
        /// Y-axis range of bingo game
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Height { get; set; }

        /// <summary>
        /// Bingo game start moment
        /// </summary>
        public DateTimeOffset? GameStart { get; set; }

        /// <summary>
        /// Bingo game end moment
        /// </summary>
        public DateTimeOffset? GameEnd { get; set; }

        /// <summary>
        /// Set true for this game being pre-deploy when calling <c>CreatePresetBingoGames()</c>
        /// </summary>
        public bool Preset { get; set; } = false;
    }
}
