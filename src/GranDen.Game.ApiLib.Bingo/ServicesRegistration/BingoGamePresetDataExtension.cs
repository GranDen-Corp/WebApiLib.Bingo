using System;
using System.Collections.Generic;
using System.Linq;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Options;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.Game.ApiLib.Bingo.Services;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GranDen.Game.ApiLib.Bingo.ServicesRegistration
{
    /// <summary>
    /// Extension method for add preset bingo game data
    /// </summary>
    public static class BingoGamePresetDataExtension
    {
        
        /// <summary>
        /// Register <c>IPresetBingoGameService</c> in the <c>IServiceCollection</c>
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="bingoGameOption"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigPresetBingoGameData(this IServiceCollection serviceCollection, BingoGameOption bingoGameOption)
        {
            var bingoGameInfoDtos = bingoGameOption.Select(o =>
                new BingoGameInfoDto
                {
                    GameName = o.GameName,
                    I18nDisplayKey = o.I18nKey,
                    StartTime = o.GameStart ?? DateTimeOffset.UtcNow,
                    EndTime = o.GameEnd,
                    Enabled = true
                }).ToList();

            return ConfigPresetBingoGameData(serviceCollection, bingoGameInfoDtos);
        }
        
        /// <summary>
        /// Register <c>IPresetBingoGameService</c> in the <c>IServiceCollection</c>
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="bingoGameInfoDtos"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigPresetBingoGameData(this IServiceCollection serviceCollection,
            IEnumerable<BingoGameInfoDto> bingoGameInfoDtos)
        {
            serviceCollection.AddSingleton<IPresetBingoGameService>(provider =>
                new PresetBingoGameService {GameInfoDtos = bingoGameInfoDtos});

            return serviceCollection;
        }

        /// <summary>
        /// Add a collection of preset <c>Bingo2dGameInfo</c> entities
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="bingoGameOption"></param>
        /// <returns></returns>
        public static IServiceProvider InitPresetBingoGameData(this IServiceProvider serviceProvider, BingoGameOption bingoGameOption)
        {
            var gameInfoDtos = bingoGameOption.Select(o =>
                new BingoGameInfoDto
                {
                    GameName = o.GameName,
                    I18nDisplayKey = o.I18nKey,
                    StartTime = o.GameStart ?? DateTimeOffset.UtcNow,
                    EndTime = o.GameEnd,
                    Enabled = true
                }).ToList();

            return InitPresetBingoGameData(serviceProvider, gameInfoDtos);
        }

        /// <summary>
        /// Add a collection of preset <c>Bingo2dGameInfo</c> entities
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="bingoGameInfos">Additional Preset Data</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IServiceProvider InitPresetBingoGameData(this IServiceProvider serviceProvider, IEnumerable<BingoGameInfoDto> bingoGameInfos = null)
        {
            var bingoGameInfoRepo = serviceProvider.GetService<IBingoGameInfoRepo>();
            var presetBingoGameService = serviceProvider.GetService<IPresetBingoGameService>();
            if (presetBingoGameService != null)
            {
                var presetBingoGames = presetBingoGameService.GameInfoDtos.ToList();
                foreach (var bingoGameInfoDto in presetBingoGames)
                {
                    if (bingoGameInfoRepo.QueryBingoGames().Any(g => g.GameName == bingoGameInfoDto.GameName))
                    {
                       continue; 
                    }

                    bingoGameInfoRepo.CreateBingoGame(bingoGameInfoDto);
                }
            }

            if (bingoGameInfos == null)
            {
                return serviceProvider;
            }

            foreach (var bingoGameInfoDto in bingoGameInfos)
            {
                if (bingoGameInfoRepo.QueryBingoGames().Any(g => g.GameName == bingoGameInfoDto.GameName))
                {
                   continue;;
                }

                bingoGameInfoRepo.CreateBingoGame(bingoGameInfoDto);
            }

            return serviceProvider;
        }
    }
}
