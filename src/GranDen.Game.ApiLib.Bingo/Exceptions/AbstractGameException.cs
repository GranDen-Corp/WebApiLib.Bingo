using System;

namespace GranDen.Game.ApiLib.Bingo.Exceptions
{
    /// <summary>
    /// Abstract exception class for bingo game.
    /// </summary>
    public abstract class AbstractGameException : Exception
    {
        /// <summary>
        /// Bingo Game Name
        /// </summary>
        public string GameName { get; protected set; }

        /// <summary>
        /// Exception class constructor
        /// </summary>
        /// <param name="message"></param>
        protected AbstractGameException(string message) : base(message) 
        {
        }
    }
}
