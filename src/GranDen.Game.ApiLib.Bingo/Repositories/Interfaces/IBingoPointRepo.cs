using System.Collections.Generic;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.Models;

namespace GranDen.Game.ApiLib.Bingo.Repositories.Interfaces
{
    /// <summary>
    /// <c>BingoPoint</c> repoistory
    /// </summary>
    public interface IBingoPointRepo
    {
        /// <summary>
        /// Get candidate <c>BingoPoint</c> entities by given Bingo Name, player Id, &amp; Geo Point Id
        /// </summary>
        /// <param name="bingoGameName"></param>
        /// <param name="bingoPlayerId"></param>
        /// <param name="geoPointId"></param>
        /// <returns></returns>
        IEnumerable<BingoPoint> GetMappedBingoPoint(string bingoGameName, string bingoPlayerId, string geoPointId);

        /// <summary>
        /// Get a filtered LINQ queryable entry point of <c>BingoPoint</c> entities by given Bingo Game name &amp; player Id
        /// </summary>
        /// <param name="bingoGameName"></param>
        /// <param name="bingoPlayerId"></param>
        /// <returns></returns>
        IQueryable<BingoPoint> QueryBingoPoints(string bingoGameName, string bingoPlayerId);
    }
}
