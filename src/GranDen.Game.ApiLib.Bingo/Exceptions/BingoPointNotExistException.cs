using System;

namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Throws when specifying wrong 2D bingo point
    /// </summary>
    [Serializable]
    public class BingoPointNotExistException : AbstractGameException
    {
        /// <summary>
        /// Player Id
        /// </summary>
        public string PlayerId { get; }

        /// <summary>
        /// The Problematic Point in (x,y) tuple
        /// </summary>
        public (int X, int Y) Point { get; }

        /// <summary>
        /// Exception class constructor
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerId"></param>
        /// <param name="point"></param>
        public BingoPointNotExistException(string gameName, string playerId, (int X, int Y) point) : base(
            $"Bingo Point ({point.X} , {point.Y}) of Player {playerId} not exist in Game {gameName}")
        {
            GameName = gameName;
            PlayerId = playerId;
            Point = point;
        }
    }
}
