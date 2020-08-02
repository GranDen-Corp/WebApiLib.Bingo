namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Throws when trying to modify 
    /// </summary>
    public class GameDisabledException : AbstractGameException
    {
        /// <summary>
        /// Exception class constructor
        /// </summary>
        /// <param name="gameName"></param>
        public GameDisabledException(string gameName) : base($"Game {gameName} disabled")
        {
            GameName = gameName;
        }
    }
}
