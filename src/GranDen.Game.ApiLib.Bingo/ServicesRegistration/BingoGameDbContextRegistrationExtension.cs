using System;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Repositories;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.Game.ApiLib.Bingo.Services;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GranDen.Game.ApiLib.Bingo.ServicesRegistration
{
    /// <summary>
    /// Register DbContext Extension method
    /// </summary>
    public static class BingoGameDbContextRegistrationExtension
    {
        /// <summary>
        /// Register the <c>BingoGameDbContext</c> in the <c>IServiceCollection</c>
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="optionAction"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="optionsLifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddBingoGameDbContext(this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionAction, ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            serviceCollection.AddDbContext<BingoGameDbContext>(optionAction, contextLifetime, optionsLifetime);
            serviceCollection.AddScoped<IBingoGameInfoRepo, BingoGameInfoRepo>();
            serviceCollection.AddScoped<IBingoGamePlayerRepo, BingoGamePlayerRepo>();
            serviceCollection.AddScoped<IBingoPointRepo, BingoPointRepo>();
            serviceCollection.AddScoped<IMappingGeoPointsRepo, MappingGeoPointsRepo>();
            serviceCollection.AddScoped<IBingoGameService<string>, BingoGameService>();

            return serviceCollection;
        }
    }
}