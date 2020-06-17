using System;
using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.GameLib.Bingo;

namespace GranDen.Game.ApiLib.Bingo.Services
{
    public interface IBingoGameService<T>
    {
        ICollection<BingoGameInfoDto> GetAttendableGames(DateTimeOffset current);

        bool JoinGame(string gameName, string playerId);

        bool MarkBingoPoint(string gameName, string playerId, (int x, int y) point);

        ICollection<MarkPoint2D> GetPlayerBingoPointStatus(string gameName, string playerId);

        ICollection<T> GetAchievedBingoPrizes(string gameName, string playerId);
    }
}