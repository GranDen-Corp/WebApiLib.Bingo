using System;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Exceptions;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Options;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace GranDen.Game.ApiLib.Bingo.Repositories
{
    /// <inheritdoc />
    public class BingoGameInfoRepo : IBingoGameInfoRepo
    {
        private readonly BingoGameDbContext _bingoGameDbContext;
        private readonly IOptionsMonitor<BingoGameOption> _optionDelegate;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="bingoGameDbContext"></param>
        /// <param name="optionDelegate"></param>
        public BingoGameInfoRepo(BingoGameDbContext bingoGameDbContext, IOptionsMonitor<BingoGameOption> optionDelegate)
        {
            _bingoGameDbContext = bingoGameDbContext;
            _optionDelegate = optionDelegate;
        }

        /// <inheritdoc />
        public int CreateBingoGame(BingoGameInfoDto bingoGameInfoDto, int? maxWidth = null, int? maxHeight = null)
        {
            var existedBingoGame = _bingoGameDbContext.Bingo2dGameInfos
                .FirstOrDefault(x => x.GameName == bingoGameInfoDto.GameName);

            if (existedBingoGame != null)
            {
                throw new GameAlreadyCreatedException(bingoGameInfoDto.GameName);
            }

            int width, height;
            if (maxWidth.HasValue && maxHeight.HasValue)
            {
                width = maxWidth.Value;
                height = maxHeight.Value;
            }
            else
            {
                var bingoGameSetting = _optionDelegate.CurrentValue.First(g => g.GameName == bingoGameInfoDto.GameName);
                width = bingoGameSetting.Width;
                height = bingoGameSetting.Height;
            }

            var newBingoGame = new Bingo2dGameInfo(bingoGameInfoDto.GameName, width, height, bingoGameInfoDto.StartTime, bingoGameInfoDto.EndTime);
            _bingoGameDbContext.Bingo2dGameInfos.Add(newBingoGame);
            _bingoGameDbContext.SaveChanges();

            return newBingoGame.Id;
        }

        /// <inheritdoc />
        public bool UpdateBingoGame(BingoGameInfoDto bingoGameInfoDto)
        {
            var existedBingoGame = _bingoGameDbContext.Bingo2dGameInfos
                .FirstOrDefault(x => x.GameName == bingoGameInfoDto.GameName);

            if (existedBingoGame == null)
            {
                throw new GameNotExistException(bingoGameInfoDto.GameName);
            }

            existedBingoGame.I18nDisplayKey = bingoGameInfoDto.I18nDisplayKey;
            existedBingoGame.StartTime = bingoGameInfoDto.StartTime;
            existedBingoGame.EndTime = bingoGameInfoDto.EndTime;
            existedBingoGame.Enabled = bingoGameInfoDto.Enabled;

            var updateCount = _bingoGameDbContext.SaveChanges();

            return updateCount > 0;
        }

        /// <inheritdoc />
        public IQueryable<Bingo2dGameInfo> QueryBingoGames()
        {
            return _bingoGameDbContext.Set<Bingo2dGameInfo>();
        }

        /// <inheritdoc />
        public Bingo2dGameInfo GetByName(string gameName)
        {
            return _bingoGameDbContext.Set<Bingo2dGameInfo>().FirstOrDefault(g => g.GameName == gameName);
        }
    }
}
