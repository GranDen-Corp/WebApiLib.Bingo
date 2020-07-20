namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Throws when associated bingo settings of the game not found 
    /// </summary>
    public class GameSettingNotFoundException : AbstractGameException
    {
        /// <summary>
        /// Exception class constructor
        /// </summary>
        /// <param name="gameName"></param>
        public GameSettingNotFoundException(string gameName) : base($"Game setting associated with Game {gameName} not found")
        {
            GameName = gameName;
        }
    }
}
