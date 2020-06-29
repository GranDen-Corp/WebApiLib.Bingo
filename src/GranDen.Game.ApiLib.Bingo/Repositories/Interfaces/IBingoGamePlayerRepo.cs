using System.Collections.Generic;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.Models;

namespace GranDen.Game.ApiLib.Bingo.Repositories.Interfaces
{
    public delegate IDictionary<(int x, int y), string> GeoPointIdInitializeDelegate(string bingoGameName, string bingoPlayerId);

    public interface IBingoGamePlayerRepo
    {
        IQueryable<BingoPlayerInfo> GetBingoGamePlayer(string bingoPlayerId);

        IQueryable<BingoPlayerInfo> QueryBingoPlayer();

        BingoPlayerInfo InitBingoGamePlayerData(string bingoGameName, string bingoPlayerId, GeoPointIdInitializeDelegate  geoPointIdInitiator);

        bool ResetBingoGamePlayerData(string bingoGameName, string bingoPlayerId);
    }
}