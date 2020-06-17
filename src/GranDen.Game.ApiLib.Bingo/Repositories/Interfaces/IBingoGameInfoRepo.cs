using System.Linq;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Models;

namespace GranDen.Game.ApiLib.Bingo.Repositories.Interfaces
{
    public interface IBingoGameInfoRepo
    {
        int CreateBingoGame(BingoGameInfoDto bingoGameInfoDto);

        bool UpdateBingoGame(BingoGameInfoDto bingoGameInfoDto);

        IQueryable<Bingo2dGameInfo> QueryBingoGames();

        Bingo2dGameInfo GetByName(string gameName);
    }
}