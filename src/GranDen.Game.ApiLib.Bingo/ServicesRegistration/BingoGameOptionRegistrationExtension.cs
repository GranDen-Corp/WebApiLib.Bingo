using System;
using GranDen.Game.ApiLib.Bingo.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GranDen.Game.ApiLib.Bingo.ServicesRegistration
{
    /// <summary>
    /// Extension methods to configure <c>BingoGameOption</c> & <c>BingoGameTable</c> options
    /// </summary>
    public static class BingoGameOptionRegistrationExtension
    {
        /// <summary>
        /// Configure <c>BingoGameOption</c> service registration
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureBingoGameOption(this IServiceCollection serviceCollection,
            IConfigurationSection configurationSection)
        {
            serviceCollection.AddOptions<BingoGameOption>().Bind(configurationSection).ValidateDataAnnotations();
            return serviceCollection;
        }

        /// <summary>
        /// Configure <c>BingoGameTableOption</c> service registration
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureBingoGameTableOption(this IServiceCollection serviceCollection,
            IConfigurationSection configurationSection)
        {
            serviceCollection.AddOptions<BingoGameTableOption>().Bind(configurationSection).ValidateDataAnnotations();
            return serviceCollection;
        }
    }
}