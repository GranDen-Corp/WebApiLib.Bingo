using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Exceptions;
using GranDen.Game.ApiLib.Bingo.Models;
using GranDen.Game.ApiLib.Bingo.Options;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;
using GranDen.Game.ApiLib.Bingo.ServicesRegistration;
using GranDen.TimeLib.ClockShaft;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GranDen.Game.ApiLib.Bingo.Test
{
    public class ErrorHandlingTest : IDisposable
    {
        #region Constant Definition

        private const string PresetBingoGameName = "DemoGame";

        private const string BingoGameOptionJsonStr = @"
{
    ""BingoGame"" : [
        {
            ""GameName"" : ""DemoGame"",
            ""GameTableKey"" : ""game_table_1"",
            ""Width"" : 3,
            ""Height"" : 3,
            ""Preset"" : true
        },
        {
            ""GameName"" : ""testGame"",
            ""GameTableKey"" : ""game_table_1"",
            ""Width"" : 3,
            ""Height"" : 3
        }
    ],
    ""GameTable"" : [
        {
            ""GameTableKey"" : ""game_table_1"",
            ""PrizeLines"" : [
                ""(0, 0), (1, 0), (2, 0) | 'Horizontal Line1'"",
                ""(0, 1), (1, 1), (2, 1) | 'Horizontal Line2'"",
                ""(0, 2), (1, 2), (2, 2) | 'Horizontal Line3'"",

                ""(0, 0), (0, 1), (0, 2) | 'Vertical Line1'"",
                ""(1, 0), (1, 1), (1, 2) | 'Vertical Line2'"",
                ""(2, 0), (2, 1), (2, 2) | 'Vertical Line3'"",

                ""(0, 0), (1, 1), (2, 2) | 'Diagonal Line1'"",
                ""(2, 0), (1, 1), (0, 2) | 'Diagonal Line2'""
            ]
        }
    ]
}
";

        #endregion

        private readonly DbConnection _connection;
        private readonly IConfigurationRoot _configuration;
        private readonly ServiceProvider _rootServiceProvider;

        public ErrorHandlingTest()
        {
            _connection = CreateInMemoryDatabase();
            _configuration = InitConfiguration();
            _rootServiceProvider = InitDependencyService();
            PresetData();
        }

        [Fact]
        public void CreateDuplicateBingoGame_Cause_GameAlreadyCreatedException()
        {
            //Arrange
            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var bingoGameInfoRepo = serviceProvider.GetService<IBingoGameInfoRepo>();

            //Assert
            var ex = Assert.Throws<GameAlreadyCreatedException>(() =>
            {
                //Act
                bingoGameInfoRepo.CreateBingoGame(
                    new BingoGameInfoDto {GameName = PresetBingoGameName, StartTime = ClockWork.DateTimeOffset.Now}, 4, 4);
            });

            Assert.IsType<GameAlreadyCreatedException>(ex);
            Assert.Equal(PresetBingoGameName, ex.GameName);
        }

        [Fact]
        public void PlayerShouldNotAddBingoRecordsAfterGameExpired()
        {
            //Arrange
            const string testPlayerId = "test_player_1";
            const string bingoGameName = "testGame";

            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var bingoGameInfoRepo = serviceProvider.GetService<IBingoGameInfoRepo>();

            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();
            var bingoGamePlayerRepo = serviceProvider.GetService<IBingoGamePlayerRepo>();

            ClockWork.ShaftConfigurationFunc = shaft =>
            {
                shaft.Backward = true;
                shaft.ShiftTimeSpan = new TimeSpan(0, 1, 0, 0);
                return shaft;
            };

            var startTime = ClockWork.DateTimeOffset.UtcNow;
            var endTime = startTime + TimeSpan.FromMinutes(30.0);

            //Act
            var gameId = bingoGameInfoRepo.CreateBingoGame(
                new BingoGameInfoDto
                {
                    GameName = bingoGameName, I18nDisplayKey = "ui_key1", StartTime = startTime, EndTime = endTime
                },
                3, 3);

            //Assert
            Assert.NotEqual(0, gameId);
            ClockWork.Reset();
            var ex = Assert.Throws<GameExpiredException>(() =>
            {
                bingoGameService.JoinGame(bingoGameName, testPlayerId);
            });

            Assert.IsType<GameExpiredException>(ex);
            Assert.Equal(bingoGameName, ex.GameName);

            var bingoPlayer = bingoGamePlayerRepo.QueryBingoPlayer().Include(p => p.JoinedGames)
                .FirstOrDefault(p => p.PlayerId == testPlayerId);
            Assert.Null(bingoPlayer);
        }

        [Fact]
        public void PlayerCanGetPrizeStatusAfterGameExpired()
        {
            //Arrange
            const string testPlayer1 = "test_player_1";
            const string testPlayer2 = "test_player_2";
            const string bingoGameName = "testGame";

            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var bingoGameInfoRepo = serviceProvider.GetService<IBingoGameInfoRepo>();

            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            ClockWork.ShaftConfigurationFunc = shaft =>
            {
                shaft.Backward = true;
                shaft.ShiftTimeSpan = new TimeSpan(0, 1, 0, 0);
                return shaft;
            };

            var startTime = ClockWork.DateTimeOffset.UtcNow;
            var endTime = startTime + TimeSpan.FromMinutes(30.0);

            var gameId = bingoGameInfoRepo.CreateBingoGame(
                new BingoGameInfoDto
                {
                    GameName = bingoGameName, I18nDisplayKey = "ui_key1", StartTime = startTime, EndTime = endTime
                },
                3, 3);

            Assert.NotEqual(0, gameId);

            Assert.True(bingoGameService.JoinGame(bingoGameName, testPlayer1));
            Assert.True(bingoGameService.MarkBingoPoint(bingoGameName, testPlayer1, (0, 0), ClockWork.DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(bingoGameName, testPlayer1, (0, 1), ClockWork.DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(bingoGameName, testPlayer1, (0, 2), ClockWork.DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(bingoGameName, testPlayer1, (1, 1), ClockWork.DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(bingoGameName, testPlayer1, (2, 2), ClockWork.DateTimeOffset.UtcNow));

            //Act
            ClockWork.Reset();

            //Assert
            var ex = Assert.Throws<GameExpiredException>(() =>
            {
                bingoGameService.JoinGame(bingoGameName, testPlayer2);
            });

            Assert.IsType<GameExpiredException>(ex);
            Assert.Equal(bingoGameName, ex.GameName);

            var markedPoints = bingoGameService.GetPlayerBingoPointStatus(bingoGameName, testPlayer1);
            Assert.Collection(markedPoints,
                point2D =>
                {
                    var (x, y, marked) = point2D;
                    Assert.Equal(0, x);
                    Assert.Equal(0, y);
                    Assert.True(marked);
                },
                point2D =>
                {
                    var (x, y, marked) = point2D;
                    Assert.Equal(0, x);
                    Assert.Equal(1, y);
                    Assert.True(marked);
                },
                point2D =>
                {
                    var (x, y, marked) = point2D;
                    Assert.Equal(0, x);
                    Assert.Equal(2, y);
                    Assert.True(marked);
                },
                point2D =>
                {
                    var (x, y, marked) = point2D;
                    Assert.Equal(1, x);
                    Assert.Equal(0, y);
                    Assert.False(marked);
                },
                point2D =>
                {
                    var (x, y, marked) = point2D;
                    Assert.Equal(1, x);
                    Assert.Equal(1, y);
                    Assert.True(marked);
                },
                point2D =>
                {
                    var (x, y, marked) = point2D;
                    Assert.Equal(1, x);
                    Assert.Equal(2, y);
                    Assert.False(marked);
                },
                point2D =>
                {
                    var (x, y, marked) = point2D;
                    Assert.Equal(2, x);
                    Assert.Equal(0, y);
                    Assert.False(marked);
                },
                point2D =>
                {
                    var (x, y, marked) = point2D;
                    Assert.Equal(2, x);
                    Assert.Equal(1, y);
                    Assert.False(marked);
                },
                point2D =>
                {
                    var (x, y, marked) = point2D;
                    Assert.Equal(2, x);
                    Assert.Equal(2, y);
                    Assert.True(marked);
                }
            );
            var bingoPrizes = bingoGameService.GetAchievedBingoPrizes(bingoGameName, testPlayer1);
            Assert.Collection(bingoPrizes,
                prizeName => Assert.Equal("Vertical Line1", prizeName),
                prizeName => Assert.Equal("Diagonal Line1", prizeName)
            );
        }

        [Fact]
        public void NotCreatedGame_Call_GetAchievedBingoPrizes_Cause_GameNotExistException()
        {
            //Arrange
            const string bingoGameName = "testGame";
            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            //Assert
            var ex = Assert.Throws<GameNotExistException>(() =>
            {
                //Act
                bingoGameService.GetAchievedBingoPrizes(bingoGameName, "test_player_id");
            });

            Assert.IsType<GameNotExistException>(ex);
            Assert.Equal(bingoGameName, ex.GameName);
        }

        [Fact]
        public void NotCreatedGame_Call_GetPlayerBingoPointStatus_Cause_GameNotExistException()
        {
            //Arrange
            const string bingoGameName = "testGame";
            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            //Assert
            var ex = Assert.Throws<GameNotExistException>(() =>
            {
                //Act
                bingoGameService.GetPlayerBingoPointStatus(bingoGameName, "test_player_id");
            });

            Assert.IsType<GameNotExistException>(ex);
            Assert.Equal(bingoGameName, ex.GameName);
        }

        [Fact]
        public void NotCreatedGame_Call_MarkBingoPoint_Cause_GameNotExistException()
        {
            //Arrange
            const string bingoGameName = "testGame";
            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            //Assert
            var ex = Assert.Throws<GameNotExistException>(() =>
            {
                //Act
                bingoGameService.MarkBingoPoint(bingoGameName, "test_player_id", (0, 0), DateTimeOffset.Now);
            });

            Assert.IsType<GameNotExistException>(ex);
            Assert.Equal(bingoGameName, ex.GameName);
        }

        [Fact]
        public void NotJoinedGame_Call_GetAchievedBingoPrizes_Cause_PlayerNotJoinedGameException()
        {
            //Arrange
            const string testPlayerId = "test_player_1";
            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            //Assert
            var ex = Assert.Throws<PlayerNotJoinedGameException>(() =>
            {
                //Act
                bingoGameService.GetAchievedBingoPrizes(PresetBingoGameName, testPlayerId);
            });

            Assert.IsType<PlayerNotJoinedGameException>(ex);
            Assert.Equal(testPlayerId, ex.PlayerId);
            Assert.Equal(PresetBingoGameName, ex.GameName);
        }

        [Fact]
        public void NotJoinedGame_Call_GetPlayerBingoPointStatus_Cause_PlayerNotJoinedGameException()
        {
            //Arrange
            const string testPlayerId = "test_player_1";
            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            //Assert
            var ex = Assert.Throws<PlayerNotJoinedGameException>(() =>
            {
                //Act
                bingoGameService.GetPlayerBingoPointStatus(PresetBingoGameName, testPlayerId);
            });

            Assert.IsType<PlayerNotJoinedGameException>(ex);
            Assert.Equal(testPlayerId, ex.PlayerId);
            Assert.Equal(PresetBingoGameName, ex.GameName);
        }

        [Fact]
        public void NotJoinedGame_Call_MarkBingoPoint_Cause_PlayerNotJoinedGameException()
        {
            //Arrange
            const string testPlayerId = "test_player_1";
            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            //Assert
            var ex = Assert.Throws<PlayerNotJoinedGameException>(() =>
            {
                //Act
                bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (0, 0), DateTimeOffset.Now);
            });

            Assert.IsType<PlayerNotJoinedGameException>(ex);
            Assert.Equal(testPlayerId, ex.PlayerId);
            Assert.Equal(PresetBingoGameName, ex.GameName);
        }

        [Fact]
        public void MarkNotExistBingoPoint_Cause_BingoPointNotExistException()
        {
            //Arrange
            const string testPlayerId = "test_player_1";

            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            Assert.True(bingoGameService.JoinGame(PresetBingoGameName, testPlayerId));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (0, 0),
                ClockWork.DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (1, 0),
                ClockWork.DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (2, 0),
                ClockWork.DateTimeOffset.UtcNow));

            //Assert
            var ex = Assert.Throws<BingoPointNotExistException>(() =>
            {
                //Act
                bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (4, 4), ClockWork.DateTimeOffset.UtcNow);
            });

            Assert.IsType<BingoPointNotExistException>(ex);
            Assert.Equal(testPlayerId, ex.PlayerId);
            Assert.Equal(PresetBingoGameName, ex.GameName);
            Assert.Equal((4, 4), ex.Point);
        }

        #region Environment Setup

        private static IConfigurationRoot InitConfiguration()
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(BingoGameOptionJsonStr));
            return new ConfigurationBuilder().AddJsonStream(ms).Build();
        }

        private ServiceProvider InitDependencyService()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.ConfigureBingoGameOption(_configuration.GetSection("BingoGame"));
            serviceCollection.ConfigureBingoGameTableOption(_configuration.GetSection("GameTable"));

            serviceCollection.AddBingoGameDbContext(builder =>
            {
                builder.UseSqlite(_connection);
                builder.EnableSensitiveDataLogging();
                builder.EnableDetailedErrors();
            });

            serviceCollection.ConfigPresetGeoPointData(new[]
            {
                "geoPoint_01", "geoPoint_02", "geoPoint_03", "geoPoint_04", "geoPoint_05", "geoPoint_06", "geoPoint_07",
                "geoPoint_08", "geoPoint_09", "geoPoint_10", "geoPoint_11", "geoPoint_12", "geoPoint_13", "geoPoint_14",
                "geoPoint_15", "geoPoint_16"
            });
            serviceCollection.AddGeoPointIdProvider((bingoGameName, bingoPlayerId) =>
                new Dictionary<(int x, int y), string>
                {
                    {(0, 0), "geoPoint_01"},
                    {(0, 1), "geoPoint_02"},
                    {(0, 2), "geoPoint_03"},
                    {(1, 0), "geoPoint_04"},
                    {(1, 1), "geoPoint_05"},
                    {(1, 2), "geoPoint_06"},
                    {(2, 0), "geoPoint_07"},
                    {(2, 1), "geoPoint_08"},
                    {(2, 2), "geoPoint_09"},
                });

            var bingoGameOption = new BingoGameOption();
            _configuration.GetSection("BingoGame").Bind(bingoGameOption);
            serviceCollection.ConfigPresetBingoGameData(bingoGameOption);

            return serviceCollection.BuildServiceProvider();
        }

        private void PresetData()
        {
            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var dbContext = serviceProvider.GetService<BingoGameDbContext>();

            if (!dbContext.Database.EnsureCreated())
            {
                dbContext.Database.Migrate();
            }

            serviceProvider.InitPresetBingoGameData();
            serviceProvider.InitPresetGeoPointData();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }

        #endregion

        public void Dispose() => _connection.Dispose();
    }
}
