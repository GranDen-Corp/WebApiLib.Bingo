using System;
using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.Game.ApiLib.Bingo.Services;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GranDen.Game.ApiLib.Bingo.ServicesRegistration
{
    /// <summary>
    /// Extension method for register Bingo Point assigner
    /// </summary>
    public static class BingoGameGeoPointRegistrationExtension
    {
        /// <summary>
        /// Register a Bingo Point assigner delegate in the <c>IServiceCollection</c>
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="geoPointIdInitializeDelegate"></param>
        /// <returns></returns>
        public static IServiceCollection AddGeoPointIdProvider(this IServiceCollection serviceCollection,
            GeoPointIdInitializeDelegate geoPointIdInitializeDelegate)
        {
            //TODO: Add default GeoPointId provider?
            serviceCollection.AddSingleton<IGeoPointIdProvider>(provider =>
                new GeoPointIdProvider(geoPointIdInitializeDelegate));

            return serviceCollection;
        }

        /// <summary>
        /// Assign a collection of preset <c>MappingGeoPoint</c> entities data
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="geoPointIds"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IServiceCollection InitGeoPointData(this IServiceCollection serviceCollection,
            IEnumerable<string> geoPointIds)
        {
            //TODO: Should using EF Core's data seeding mechanism to fill those preset data?
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<BingoGameDbContext>();
            if (dbContext.Database.EnsureCreated())
            {
                dbContext.Database.Migrate();
            }

            var mappingGeoPointsRepo = serviceProvider.GetService<IMappingGeoPointsRepo>();

            if (!mappingGeoPointsRepo.CreateMappingGeoPoints(geoPointIds))
            {
                throw new Exception("Preset Bingo Game Db MappingGeoPoint failed.");
            }

            return serviceCollection;
        }
    }
}
