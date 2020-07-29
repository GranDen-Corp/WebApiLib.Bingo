namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Throws when associated bingo settings of the game not found
    /// </summary>
    public class GameTableSettingNotfoundException : AbstractGameException
    {
        /// <summary>
        /// Game Table key name
        /// </summary>
        public string GameTableKey { get; private set; }

        /// <summary>
        /// Exception class constructor
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="gameTableKey"></param>
        public GameTableSettingNotfoundException(string gameName, string gameTableKey) : base($"Game Table setting {gameTableKey} associated with Game {gameName} not found")
        {
            GameName = gameName;
            GameTableKey = gameTableKey;
        }
    }
}
