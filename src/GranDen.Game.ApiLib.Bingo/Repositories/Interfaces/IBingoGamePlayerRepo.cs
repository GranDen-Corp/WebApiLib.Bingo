using System.Collections.Generic;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.Models;

namespace GranDen.Game.ApiLib.Bingo.Repositories.Interfaces
{
    /// <summary>
    /// Defined the Bingo Point initializer lambda function type 
    /// </summary>
    /// <param name="bingoGameName"></param>
    /// <param name="bingoPlayerId"></param>
    public delegate IDictionary<(int x, int y), string> GeoPointIdInitializeDelegate(string bingoGameName, string bingoPlayerId);

    /// <summary>
    /// <c>BingoPlayerInfo</c> repository
    /// </summary>
    public interface IBingoGamePlayerRepo
    {
        /// <summary>
        /// Get LINQ queryable entry point of <c>BingoPlayerInfo</c> entity by given player Id
        /// </summary>
        /// <param name="bingoPlayerId"></param>
        /// <returns></returns>
        IQueryable<BingoPlayerInfo> GetBingoGamePlayer(string bingoPlayerId);

        /// <summary>
        /// Get LINQ queryable entry point of <c>BingoPlayerInfo</c> entities
        /// </summary>
        /// <returns></returns>
        IQueryable<BingoPlayerInfo> QueryBingoPlayer();

        /// <summary>
        /// Create a new <c>BingoPlayerInfo</c> entity
        /// </summary>
        /// <param name="bingoGameName"></param>
        /// <param name="bingoPlayerId"></param>
        /// <param name="geoPointIdInitiator"></param>
        /// <returns></returns>
        BingoPlayerInfo InitBingoGamePlayerData(string bingoGameName, string bingoPlayerId, GeoPointIdInitializeDelegate  geoPointIdInitiator);

        /// <summary>
        /// Clear Bingo Mark Point records of a given Bingo Game &amp; player Id
        /// </summary>
        /// <param name="bingoGameName"></param>
        /// <param name="bingoPlayerId"></param>
        /// <returns></returns>
        bool ResetBingoGamePlayerData(string bingoGameName, string bingoPlayerId);
    }
}
