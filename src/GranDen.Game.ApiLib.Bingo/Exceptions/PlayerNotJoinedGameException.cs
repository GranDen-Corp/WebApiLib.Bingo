using System;

namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Raise when query bingo prize (<c>GetAchievedBingoPrizes()</c>), get mark point status(<c>GetPlayerBingoPointStatus()</c>), set mark point(<c>MarkBingoPoint()</c>) when player not joined that game
    /// </summary>
    [Serializable]
    public class PlayerNotJoinedGameException : Exception
    {
        /// <summary>
        /// Bingo Game Name
        /// </summary>
        public string GameName { get; private set; }

        /// <summary>
        /// Player Id
        /// </summary>
        public string PlayerId { get; private set; }

        /// <summary>
        /// Exception Class constructor
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerId"></param>
        public PlayerNotJoinedGameException(string gameName, string playerId) : base(
            $"Player {playerId} not joined Game {gameName}")
        {
            GameName = gameName;
            PlayerId = playerId;
        }
    }
}
