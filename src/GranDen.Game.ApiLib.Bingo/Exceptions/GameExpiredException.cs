using System;

namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Throws when accessing expired game(s)
    /// </summary>
    [Serializable]
    public class GameExpiredException : AbstractGameException
    {
        /// <summary>
        /// Exception class constructor
        /// </summary>
        /// <param name="gameName"></param>
        public GameExpiredException(string gameName) : base($"Game {gameName} expired")
        {
            GameName = gameName;
        }
    }
}
