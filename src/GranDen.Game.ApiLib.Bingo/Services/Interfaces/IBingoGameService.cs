using System;
using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.GameLib.Bingo;

namespace GranDen.Game.ApiLib.Bingo.Services.Interfaces
{
    /// <summary>
    /// Bingo Game Service Interface
    /// </summary>
    /// <typeparam name="T">Bingo Line prize object</typeparam>
    public interface IBingoGameService<T>
    {
        /// <summary>
        /// Get a list of Bingo Game that can attend &amp; play
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        ICollection<BingoGameInfoDto> GetAttendableGames(DateTimeOffset current);

        /// <summary>
        /// Join a Bingo Game
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        bool JoinGame(string gameName, string playerId);

        /// <summary>
        /// Mark a Bingo point
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerId"></param>
        /// <param name="point"></param>
        /// <param name="markedTime"></param>
        /// <returns></returns>
        bool MarkBingoPoint(string gameName, string playerId, (int x, int y) point, DateTimeOffset markedTime);

        /// <summary>
        /// Mark a Bingo point
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerId"></param>
        /// <param name="bingoPointDto"></param>
        /// <returns></returns>
        bool MarkBingoPoint(string gameName, string playerId, BingoPointDto bingoPointDto);

        /// <summary>
        /// Get all player's mark point of a certain Bingo Game
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        ICollection<MarkPoint2D> GetPlayerBingoPointStatus(string gameName, string playerId);

        /// <summary>
        /// Get currently all achieved bingo line prizes
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerId"></param>
        /// <param name="tableKey"></param>
        /// <returns></returns>
        ICollection<T> GetAchievedBingoPrizes(string gameName, string playerId, string tableKey = null);

        /// <summary>
        /// Reset a player's all bingo points status of a certain Bingo Game
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        bool ResetMarkBingoPoint(string gameName, string playerId);
        
    }
}
