﻿using System;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Options;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace GranDen.Game.ApiLib.Bingo.Repositories
{
    public class BingoGameInfoRepo : IBingoGameInfoRepo
    {
        private readonly BingoGameDbContext _bingoGameDbContext;
        private readonly IOptionsMonitor<BingoGameOption> _optionDelegate;

        public BingoGameInfoRepo(BingoGameDbContext bingoGameDbContext, IOptionsMonitor<BingoGameOption> optionDelegate)
        {
            _bingoGameDbContext = bingoGameDbContext;
            _optionDelegate = optionDelegate;
        }

        public int CreateBingoGame(BingoGameInfoDto bingoGameInfoDto)
        {
            var existedBingoGame = _bingoGameDbContext.Bingo2dGameInfos
                .FirstOrDefault(x => x.GameName == bingoGameInfoDto.GameName);

            if (existedBingoGame != null)
            {
                throw new Exception($"Bingo Game {bingoGameInfoDto.GameName} already exist.");
            }

            var bingoGameSetting = _optionDelegate.CurrentValue.First(g => g.GameName == bingoGameInfoDto.GameName);

            var newBingoGame = new Bingo2dGameInfo(bingoGameInfoDto.GameName, bingoGameSetting.Width, bingoGameSetting.Height, bingoGameInfoDto.StartTime, bingoGameInfoDto.EndTime);
            _bingoGameDbContext.Bingo2dGameInfos.Add(newBingoGame);
            _bingoGameDbContext.SaveChanges();

            return newBingoGame.Id;
        }

        public bool UpdateBingoGame(BingoGameInfoDto bingoGameInfoDto)
        {
            var existedBingoGame = _bingoGameDbContext.Bingo2dGameInfos
                .FirstOrDefault(x => x.GameName == bingoGameInfoDto.GameName);

            if (existedBingoGame == null)
            {
                throw new Exception($"Bingo Game {bingoGameInfoDto.GameName} hasn't created.");
            }

            existedBingoGame.I18nDisplayKey = bingoGameInfoDto.I18nDisplayKey;
            existedBingoGame.StartTime = bingoGameInfoDto.StartTime;
            existedBingoGame.EndTime = bingoGameInfoDto.EndTime;
            existedBingoGame.Enabled = bingoGameInfoDto.Enabled;

            var updateCount = _bingoGameDbContext.SaveChanges();

            return updateCount > 0;
        }

        public IQueryable<Bingo2dGameInfo> QueryBingoGames()
        {
            return _bingoGameDbContext.Set<Bingo2dGameInfo>();
        }

        public Bingo2dGameInfo GetByName(string gameName)
        {
            return _bingoGameDbContext.Set<Bingo2dGameInfo>().FirstOrDefault(g => g.GameName == gameName);
        }
    }
}