using System;

namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Throw when no such game name exist.
    /// </summary>
    [Serializable]
    public class GameNotExistException : AbstractGameException
    {
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
