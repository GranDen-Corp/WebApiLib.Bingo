using System.Collections.Generic;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.Models;

namespace GranDen.Game.ApiLib.Bingo.Repositories.Interfaces
{
    public interface IBingoGamePlayerRepo
    {
        IQueryable<BingoPlayerInfo> GetBingoGamePlayer(string bingoPlayerId);

        IQueryable<BingoPlayerInfo> QueryBingoPlayer();

        BingoPlayerInfo InitBingoGamePlayerData(string bingoGameName, string bingoPlayerId, IDictionary<(int x, int y), string> geoPointIds);

        bool ResetBingoGamePlayerData(string bingoGameName, string bingoPlayerId);
    }
}