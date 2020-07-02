using System.Linq;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Models;

namespace GranDen.Game.ApiLib.Bingo.Repositories.Interfaces
{
    /// <summary>
    /// <c>Bingo2dGameInfo</c> repository
    /// </summary>
    public interface IBingoGameInfoRepo
    {
        /// <summary>
        /// Create new <c>Bingo2dGameInfo</c> entity
        /// </summary>
        /// <param name="bingoGameInfoDto"></param>
        /// <returns></returns>
        int CreateBingoGame(BingoGameInfoDto bingoGameInfoDto);

        /// <summary>
        /// Update a existing <c>Bingo2dGameInfo</c> entity
        /// </summary>
        /// <param name="bingoGameInfoDto"></param>
        /// <returns></returns>
        bool UpdateBingoGame(BingoGameInfoDto bingoGameInfoDto);

        /// <summary>
        /// Get LINQ queryable entry point of <c>Bingo2dGameInfo</c>
        /// </summary>
        /// <returns></returns>
        IQueryable<Bingo2dGameInfo> QueryBingoGames();

        /// <summary>
        /// Get the <c>Bingo2dGameInfo</c> entity by its Name 
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        Bingo2dGameInfo GetByName(string gameName);
    }
}
