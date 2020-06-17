using System.Linq;
using GranDen.Game.ApiLib.Bingo.Models;

namespace GranDen.Game.ApiLib.Bingo.Repositories.Interfaces
{
    public interface IBingoPointRepo
    {
       BingoPoint GetMappedBingoPoint(string bingoGameName, string bingoPlayerId, string geoPointId);

       IQueryable<BingoPoint> QueryBingoPoints(string bingoGameName, string bingoPlayerId);

    }
}