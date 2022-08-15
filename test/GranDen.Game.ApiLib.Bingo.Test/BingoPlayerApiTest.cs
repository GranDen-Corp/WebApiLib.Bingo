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
    public class BingoPlayerApiTest : IDisposable
    {
        #region Constant Definition

        private const string PresetBingoGameName = "DemoGame";

        private const string BingoGameOptionJsonStr = @"
{
    ""BingoGame"" : [
        {
            ""GameName"" : ""DemoGame"",
            ""GameTableKey"" : ""game_1"",
            ""Width"" : 4,
            ""Height"" : 4,
            ""Preset"" : true
        }
    ],
    ""GameTable"" : [
        {
            ""GameTableKey"" : ""game_1"",
            ""PrizeLines"" : [
                ""(0, 0), (1, 0), (2, 0), (3, 0) | 'Horizontal Line1'"",
                ""(0, 1), (1, 1), (2, 1), (3, 1) | 'Horizontal Line2'"",
                ""(0, 2), (1, 2), (2, 2), (3, 2) | 'Horizontal Line3'"",
                ""(0, 3), (1, 3), (2, 3), (3, 3) | 'Horizontal Line4'"",

                ""(0, 0), (0, 1), (0, 2), (0, 3) | 'Vertical Line1'"",
                ""(1, 0), (1, 1), (1, 2), (1, 3) | 'Vertical Line2'"",
                ""(2, 0), (2, 1), (2, 2), (2, 3) | 'Vertical Line3'"",
                ""(3, 0), (3, 1), (3, 2), (3, 3) | 'Vertical Line4'"",

                ""(0, 0), (1, 1), (2, 2), (3, 3) | 'Diagonal Line1'"",
                ""(3, 0), (2, 1), (1, 2), (0, 3) | 'Diagonal Line2'""
            ]
        }
    ]
}
";

        #endregion

        private readonly DbConnection _connection;
        private readonly IConfigurationRoot _configuration;
        private readonly ServiceProvider _rootServiceProvider;

        public BingoPlayerApiTest()
        {
            _connection = CreateInMemoryDatabase();
            _configuration = InitConfiguration();
            _rootServiceProvider = InitServices();
            PresetData();
        }

        [Fact]
        public void PlayerShouldBeAbleToJoinNonExpiredGame()
        {
            //Arrange
            const string testPlayerId = "test_player_1";
            const string bingoGameName = "testGame";

            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var bingoGameInfoRepo = serviceProvider.GetService<IBingoGameInfoRepo>();

            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();
            var bingoGamePlayerRepo = serviceProvider.GetService<IBingoGamePlayerRepo>();

            //Act
            Assert.True(bingoGameInfoRepo != null);
            Assert.True(bingoGameService != null);
            var gameId = bingoGameInfoRepo.CreateBingoGame(
                new BingoGameInfoDto
                {
                    GameName = bingoGameName, I18nDisplayKey = "ui_key1", StartTime = ClockWork.DateTimeOffset.UtcNow
                },
                4, 4);

            var attendableGames = bingoGameService.GetAttendableGames(ClockWork.DateTimeOffset.UtcNow);

            var joined = bingoGameService.JoinGame(bingoGameName, testPlayerId);

            //Assert
            Assert.NotEmpty(attendableGames);

            Assert.NotEqual(0, gameId);
            Assert.True(joined);

            Assert.True(bingoGamePlayerRepo != null);
            var bingoPlayer = bingoGamePlayerRepo.QueryBingoPlayer().Include(p => p.JoinedGames)
                .FirstOrDefault(p => p.PlayerId == testPlayerId);
            Assert.NotNull(bingoPlayer);
            var joinedGame = bingoPlayer.JoinedGames.FirstOrDefault(g => g.GameName == bingoGameName);
            Assert.NotNull(joinedGame);
            Assert.Equal(bingoGameName, joinedGame.GameName);
        }

        [Fact]
        public void CanGetCorrectMarked2DPoint()
        {
            //Arrange
            const string testPlayerId = "test_player_1";
            const string testGeoPointId = "geoPoint_07";

            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            Assert.True(bingoGameService != null && bingoGameService.JoinGame(PresetBingoGameName, testPlayerId));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, new BingoPointDto {X = 1, Y = 2}));

            //Act
            var bingoPointRepo = _rootServiceProvider.GetService<IBingoPointRepo>();
            var candidateBingoPoints = bingoPointRepo.GetMappedBingoPoint(PresetBingoGameName, testPlayerId, testGeoPointId);

            //Assert
            var bingoPoints = candidateBingoPoints.ToList();
            Assert.Single(bingoPoints);
            var thePoint = bingoPoints.First();
            Assert.Equal(PresetBingoGameName, thePoint.BelongingGame.GameName);
            Assert.Equal(testPlayerId, thePoint.BelongingPlayer.PlayerId);
            Assert.Equal(1, thePoint.MarkPoint.X);
            Assert.Equal(2, thePoint.MarkPoint.Y);
            Assert.True(thePoint.MarkPoint.Marked);
        }

        [Fact]
        public void CanResetPLayerMarked2DPointStatus()
        {
            //Arrange
            const string testPlayerId = "test_player_1";

            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();
            var bingoPointRepo = serviceProvider.GetService<IBingoPointRepo>();

            Assert.True(bingoGameService != null && bingoGameService.JoinGame(PresetBingoGameName, testPlayerId));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, new BingoPointDto {X = 0, Y = 0}));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, new BingoPointDto {X = 1, Y = 1}));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, new BingoPointDto {X = 1, Y = 2}));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, new BingoPointDto {X = 2, Y = 2}));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, new BingoPointDto {X = 3, Y = 3}));

            //Act
            Assert.True(bingoGameService.ResetMarkBingoPoint(PresetBingoGameName, testPlayerId));

            //Assert
            var bingoPoints = bingoPointRepo.QueryBingoPoints(PresetBingoGameName, testPlayerId).OrderBy(p => p.MarkPoint.X)
                .ThenBy(p => p.MarkPoint.Y).ToList();

            foreach (var markedPoint in bingoPoints)
            {
                Assert.False(markedPoint.MarkPoint.Marked);
                Assert.Null(markedPoint.ClearTime);
            }
        }

        [Fact]
        public void PlayerCanAddBingoPointStatusDuringValidGamePeriod()
        {
            //Arrange
            const string testPlayerId = "test_player_1";

            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            Assert.True(bingoGameService != null && bingoGameService.JoinGame(PresetBingoGameName, testPlayerId));

            //Act
            var marked0 = bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, new BingoPointDto {X = 0, Y = 0});
            var marked1 = bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, new BingoPointDto {X = 0, Y = 1});
            var marked2 = bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, new BingoPointDto {X = 0, Y = 2});
            var markedPoints = bingoGameService.GetPlayerBingoPointStatus(PresetBingoGameName, testPlayerId)
                .Where(p => p.Marked).OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            //Assert
            Assert.True(marked0);
            Assert.True(marked1);
            Assert.True(marked2);
            Assert.Equal(3, markedPoints.Count);
            Assert.Collection(markedPoints,
                m =>
                {
                    Assert.Equal(0, m.X);
                    Assert.Equal(0, m.Y);
                    Assert.True(m.Marked);
                },
                m =>
                {
                    Assert.Equal(0, m.X);
                    Assert.Equal(1, m.Y);
                    Assert.True(m.Marked);
                },
                m =>
                {
                    Assert.Equal(0, m.X);
                    Assert.Equal(2, m.Y);
                    Assert.True(m.Marked);
                }
            );

            //Get inner DB data to exam point belonging owner
            var bingoPointRepo = _rootServiceProvider.GetService<IBingoPointRepo>();

            var bingoPoints = bingoPointRepo.QueryBingoPoints(PresetBingoGameName, testPlayerId)
                .Include(m => m.BelongingGame).Include(m => m.BelongingPlayer)
                .Where(p => p.MarkPoint.Marked).OrderBy(p => p.MarkPoint.X).ThenBy(p => p.MarkPoint.Y).ToList();

            Assert.Equal(3, markedPoints.Count);

            Assert.Collection(bingoPoints,
                m =>
                {
                    Assert.Equal(PresetBingoGameName, m.BelongingGame.GameName);
                    Assert.Equal(testPlayerId, m.BelongingPlayer.PlayerId);
                    Assert.Equal(0, m.MarkPoint.X);
                    Assert.Equal(0, m.MarkPoint.Y);
                },
                m =>
                {
                    Assert.Equal(PresetBingoGameName, m.BelongingGame.GameName);
                    Assert.Equal(testPlayerId, m.BelongingPlayer.PlayerId);
                    Assert.Equal(0, m.MarkPoint.X);
                    Assert.Equal(1, m.MarkPoint.Y);
                },
                m =>
                {
                    Assert.Equal(PresetBingoGameName, m.BelongingGame.GameName);
                    Assert.Equal(testPlayerId, m.BelongingPlayer.PlayerId);
                    Assert.Equal(0, m.MarkPoint.X);
                    Assert.Equal(2, m.MarkPoint.Y);
                }
            );
        }

        [Fact]
        public void PlayerCanGetPrizeStatusBeforeGameExpired()
        {
            //Arrange
            const string testPlayerId = "test_player_1";

            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            Assert.True(bingoGameService.JoinGame(PresetBingoGameName, testPlayerId));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (0, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (1, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (2, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (3, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (0, 1), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (0, 2), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (0, 3), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (1, 1), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (2, 2), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (3, 3), DateTimeOffset.UtcNow));

            //Act
            var achievedPrizes = bingoGameService.GetAchievedBingoPrizes(PresetBingoGameName, testPlayerId);

            //Assert
            Assert.Equal(3, achievedPrizes.Count);
            Assert.Collection(achievedPrizes,
                m =>
                {
                    Assert.Equal("Horizontal Line1", m);
                },
                m =>
                {
                    Assert.Equal("Vertical Line1", m);
                },
                m =>
                {
                    Assert.Equal("Diagonal Line1", m);
                });
        }

        [Fact]
        public void PlayerLeaveGameWillClearBingoGameData()
        {
            //Arrange
            const string testPlayerId = "test_player_1";

            using var serviceScope = _rootServiceProvider.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var bingoGameInfoRepo = serviceProvider.GetService<IBingoGameInfoRepo>();
            var bingoGamePlayerRepo = serviceProvider.GetService<IBingoGamePlayerRepo>();
            var bingoPointRepo = serviceProvider.GetService<IBingoPointRepo>();
            var bingoGameService = serviceProvider.GetService<I2DBingoGameService>();

            Assert.True(bingoGameService.JoinGame(PresetBingoGameName, testPlayerId));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (0, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (1, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (2, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(PresetBingoGameName, testPlayerId, (3, 0), DateTimeOffset.UtcNow));

            //Act
            var isSuccessfulLeave = bingoGameService.LeaveGame(PresetBingoGameName, testPlayerId);

            //Assert
            Assert.True(isSuccessfulLeave);

            var bingoGame = bingoGameInfoRepo.QueryBingoGames().Include(g => g.JoinedPlayers)
                .FirstOrDefault(g => g.GameName == PresetBingoGameName);
            Assert.NotNull(bingoGame);
            Assert.True(bingoGame.JoinedPlayers.All(p => p.PlayerId == testPlayerId));

            Assert.Null(bingoGamePlayerRepo.GetBingoGamePlayer(testPlayerId).FirstOrDefault());

            var ex = Assert.Throws<PlayerNotJoinedGameException>(() =>
            {
                bingoPointRepo.QueryBingoPoints(PresetBingoGameName, testPlayerId);
            });

            Assert.IsType<PlayerNotJoinedGameException>(ex);
            Assert.Equal(PresetBingoGameName, ex.GameName);
            Assert.Equal(testPlayerId, ex.PlayerId);
        }

        #region Environment Setup

        private static IConfigurationRoot InitConfiguration()
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(BingoGameOptionJsonStr));
            return new ConfigurationBuilder().AddJsonStream(ms).Build();
        }

        private ServiceProvider InitServices()
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
                    {(0, 3), "geoPoint_04"},
                    {(1, 0), "geoPoint_05"},
                    {(1, 1), "geoPoint_06"},
                    {(1, 2), "geoPoint_07"},
                    {(1, 3), "geoPoint_08"},
                    {(2, 0), "geoPoint_09"},
                    {(2, 1), "geoPoint_10"},
                    {(2, 2), "geoPoint_11"},
                    {(2, 3), "geoPoint_12"},
                    {(3, 0), "geoPoint_13"},
                    {(3, 1), "geoPoint_14"},
                    {(3, 2), "geoPoint_15"},
                    {(3, 3), "geoPoint_16"},
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

            //NOTE: doesn't know why in-memory SQLite DB cannot just call Migration() to create DB Schema
            if (!dbContext.Database.EnsureCreated())
            {
                dbContext.Database.Migrate();
            }

            serviceProvider.InitPresetGeoPointData();
            serviceProvider.InitPresetBingoGameData();
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
