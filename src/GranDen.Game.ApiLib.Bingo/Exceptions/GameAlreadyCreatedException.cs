namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Throws when trying to create Bingo Game that already exist. 
    /// </summary>
    public class GameAlreadyCreatedException : AbstractGameException
    {
        /// <summary>
        /// Exception class constructor
        /// </summary>
        /// <param name="gameName"></param>
        public GameAlreadyCreatedException(string gameName) : base($"Bingo Game {gameName} already exist.")
        {
            GameName = gameName;
        }
    }
}
