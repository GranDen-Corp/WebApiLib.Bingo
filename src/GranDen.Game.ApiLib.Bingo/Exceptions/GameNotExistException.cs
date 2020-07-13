using System;

namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Throw when query bingo prize (<c>GetAchievedBingoPrizes()</c>), get mark point status(<c>GetPlayerBingoPointStatus()</c>), set mark point(<c>MarkBingoPoint()</c>) that no such game exist.
    /// </summary>
    [Serializable]
    public class GameNotExistException : Exception
    {
        /// <summary>
        /// Bingo Game Name
        /// </summary>
        public string GameName { get; private set; }

        /// <summary>
        /// Exception class constructor
        /// </summary>
        /// <param name="gameName"></param>
        public GameNotExistException(string gameName) : base($"No such Game {gameName}")
        {
            GameName = gameName;
        }
    }
}
