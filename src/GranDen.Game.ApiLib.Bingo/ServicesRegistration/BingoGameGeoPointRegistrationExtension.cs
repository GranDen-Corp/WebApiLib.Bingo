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
    public static class BingoGameGeoPointRegistrationExtension
    {
        public static IServiceCollection AddGeoPointIdProvider(this IServiceCollection serviceCollection,
            GeoPointIdInitializeDelegate geoPointIdInitializeDelegate)
        {
            //TODO: Add default GeoPointId provider?
            serviceCollection.AddSingleton<IGeoPointIdProvider>(provider =>
                new GeoPointIdProvider(geoPointIdInitializeDelegate));

            return serviceCollection;
        }

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