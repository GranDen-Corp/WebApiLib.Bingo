using System;
using System.Collections.Generic;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.Game.ApiLib.Bingo.Services;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GranDen.Game.ApiLib.Bingo.ServicesRegistration
{
    /// <summary>
    /// Extension methods for register Bingo Point assigner
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
        /// Register <c>IPresetGeoPointService</c> in the <c>IServiceCollection</c>
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="geoPointIds"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigPresetGeoPointData(this IServiceCollection serviceCollection,
            IEnumerable<string> geoPointIds)
        {
            serviceCollection.AddSingleton<IPresetGeoPointService>(provider =>
                new PresetGeoPointService {GeoPoints = geoPointIds});

            return serviceCollection;
        }

        /// <summary>
        /// Assign a collection of preset <c>MappingGeoPoint</c> entities 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="geoPointIds">Additional Preset Data</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IServiceProvider InitPresetGeoPointData(this IServiceProvider serviceProvider,
            IEnumerable<string> geoPointIds = null)
        {
            var mappingGeoPointsRepo = serviceProvider.GetService<IMappingGeoPointsRepo>();
            var presetGeoPointService = serviceProvider.GetService<IPresetGeoPointService>();
            if (presetGeoPointService != null)
            {
                var presetGeoPointData = serviceProvider.GetService<IPresetGeoPointService>().GeoPoints;

                if (!mappingGeoPointsRepo.CreateMappingGeoPoints(presetGeoPointData))
                {
                    throw new Exception("Preset Bingo Game Db MappingGeoPoint failed.");
                }
            }

            if (geoPointIds == null)
            {
                return serviceProvider;
            }

            if (!mappingGeoPointsRepo.CreateMappingGeoPoints(geoPointIds))
            {
                throw new Exception("Preset Bingo Game Db MappingGeoPoint failed.");
            }

            return serviceProvider;
        }
    }
}
