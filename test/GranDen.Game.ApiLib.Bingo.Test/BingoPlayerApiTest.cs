using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using GranDen.Game.ApiLib.Bingo.DTO;
using GranDen.Game.ApiLib.Bingo.Repositories.Interfaces;
using GranDen.Game.ApiLib.Bingo.Services.Interfaces;
using GranDen.Game.ApiLib.Bingo.ServicesRegistration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GranDen.Game.ApiLib.Bingo.Test
{
    public class BingoPlayerApiTest : IDisposable
    {
        private readonly DbConnection _connection;
        private IConfigurationRoot _configuration;
        private ServiceProvider _serviceProvider;

        public BingoPlayerApiTest()
        {
            _connection = CreateInMemoryDatabase();

            InitConfiguration();
            InitServices();
        }


        [Fact]
        public void PlayerShouldBeAbleToJoinNonExpiredGame()
        {
            //Arrange
            const string TestPlayerId = "test_player_1";
            const string BingoGameName = "DemoGame";
            var bingoGameInfoRepo = _serviceProvider.GetService<IBingoGameInfoRepo>();

            var bingoGameService = _serviceProvider.GetService<IBingoGameService<string>>();
            var bingoGamePlayerRepo = _serviceProvider.GetService<IBingoGamePlayerRepo>();

            //Act
            var gameId = bingoGameInfoRepo.CreateBingoGame(new BingoGameInfoDto
                {GameName = BingoGameName, I18nDisplayKey = "ui_key1", StartTime = DateTimeOffset.UtcNow});

            var joined = bingoGameService.JoinGame(BingoGameName, TestPlayerId);

            //Assert
            Assert.NotEqual(0, gameId);
            Assert.True(joined);

            var bingoPlayer = bingoGamePlayerRepo.QueryBingoPlayer().Include(p => p.JoinedGames)
                .FirstOrDefault(p => p.PlayerId == TestPlayerId);
            Assert.NotNull(bingoPlayer);
            var joinedGame = bingoPlayer.JoinedGames.FirstOrDefault(g => g.GameName == BingoGameName);
            Assert.NotNull(joinedGame);
            Assert.Equal(BingoGameName, joinedGame.GameName);
        }

        [Fact]
        public void PlayerCanAddBingoPointStatusDuringValidGamePeriod()
        {
            //Arrange
            const string TestPlayerId = "test_player_1";
            const string BingoGameName = "DemoGame";
            var bingoGameInfoRepo = _serviceProvider.GetService<IBingoGameInfoRepo>();

            var bingoGameService = _serviceProvider.GetService<IBingoGameService<string>>();

            bingoGameInfoRepo.CreateBingoGame(new BingoGameInfoDto
                {GameName = BingoGameName, I18nDisplayKey = "ui_key1", StartTime = DateTimeOffset.UtcNow});

            Assert.True(bingoGameService.JoinGame(BingoGameName, TestPlayerId));
            
            //Act
            var marked0 = bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, new BingoPointDto { X = 0, Y = 0 });
            var marked1 = bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, new BingoPointDto { X = 0, Y = 1 });
            var marked2 = bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, new BingoPointDto { X = 0, Y = 2 });

            //Assert
            Assert.True(marked0);
            Assert.True(marked1);
            Assert.True(marked2);
            var bingoPointRepo = _serviceProvider.GetService<IBingoPointRepo>();
            var markedPoints = bingoPointRepo.QueryBingoPoints(BingoGameName, TestPlayerId)
                .Include(m => m.BelongingGame).Include(m => m.BelongingPlayer).Where(p => p.MarkPoint.Marked).ToList();

            Assert.Equal(3, markedPoints.Count);

            Assert.Collection(markedPoints,
                m =>
                {
                    Assert.Equal(BingoGameName, m.BelongingGame.GameName);
                    Assert.Equal(TestPlayerId, m.BelongingPlayer.PlayerId);
                    Assert.Equal(0, m.MarkPoint.X);
                    Assert.Equal(2, m.MarkPoint.Y);
                },
                
                m =>
                {
                    Assert.Equal(BingoGameName, m.BelongingGame.GameName);
                    Assert.Equal(TestPlayerId, m.BelongingPlayer.PlayerId);
                    Assert.Equal(0, m.MarkPoint.X);
                    Assert.Equal(1, m.MarkPoint.Y);
                },
                m =>
                {
                    Assert.Equal(BingoGameName, m.BelongingGame.GameName);
                    Assert.Equal(TestPlayerId, m.BelongingPlayer.PlayerId);
                    Assert.Equal(0, m.MarkPoint.X);
                    Assert.Equal(0, m.MarkPoint.Y);
                }
            );
        }

        [Fact]
        public void PlayerCanGetPrizeStatusBeforeGameExpired()
        {
            
            //Arrange
            const string TestPlayerId = "test_player_1";
            const string BingoGameName = "DemoGame";
            var bingoGameInfoRepo = _serviceProvider.GetService<IBingoGameInfoRepo>();

            var bingoGameService = _serviceProvider.GetService<IBingoGameService<string>>();

            bingoGameInfoRepo.CreateBingoGame(new BingoGameInfoDto
                {GameName = BingoGameName, I18nDisplayKey = "ui_key1", StartTime = DateTimeOffset.UtcNow});

            Assert.True(bingoGameService.JoinGame(BingoGameName, TestPlayerId));
            Assert.True(bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, (0, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, (1, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, (2, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, (3, 0), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, (0, 1), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, (0, 2), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, (0, 3), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, (1, 1), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, (2, 2), DateTimeOffset.UtcNow));
            Assert.True(bingoGameService.MarkBingoPoint(BingoGameName, TestPlayerId, (3, 3), DateTimeOffset.UtcNow));
            
            //Act
            var achievedPrizes = bingoGameService.GetAchievedBingoPrizes(BingoGameName, TestPlayerId);
            
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

        [Fact(Skip = "TBD")]
        public void PlayerShouldNotAddBingoRecordsAfterGameExpired()
        {
        }

        [Fact(Skip = "TBD")]
        public void PlayerCanGetPrizeStatusAfterGameExpired()
        {
        }

        #region Environment Setup


        private void InitConfiguration()
        {

            const string BingoGameJsonStr = @"
{
    ""BingoGame"" : [
        {
            ""GameName"" : ""DemoGame"",
            ""GameTableKey"" : ""game_1"",
            ""Width"" : 4,
            ""Height"" : 4
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

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(BingoGameJsonStr));
            _configuration = new ConfigurationBuilder().AddJsonStream(ms).Build();
        }

        private void InitServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.ConfigureBingoGameOption(_configuration.GetSection("BingoGame"));
            serviceCollection.ConfigureBingoGameTableOption(_configuration.GetSection("GameTable"));

            serviceCollection.AddBingoGameDbContext(builder =>
            {
                builder.UseSqlite(_connection);
            });
            serviceCollection.InitGeoPointData(new []
            {
                "geoPoint_01",
                "geoPoint_02",
                "geoPoint_03",
                "geoPoint_04",
                "geoPoint_05",
                "geoPoint_06",
                "geoPoint_07",
                "geoPoint_08",
                "geoPoint_09",
                "geoPoint_10",
                "geoPoint_11",
                "geoPoint_12",
                "geoPoint_13",
                "geoPoint_14",
                "geoPoint_15",
                "geoPoint_16"
            });
            serviceCollection.AddGeoPointIdProvider((bingoGameName, bingoPlayerId) => 
                new Dictionary<(int x, int y), string>
                {
                    {(0,0), "geoPoint_01"},
                    {(0,1), "geoPoint_02"},
                    {(0,2), "geoPoint_03"},
                    {(0,3), "geoPoint_04"},
                    {(1,0), "geoPoint_05"},
                    {(1,1), "geoPoint_06"},
                    {(1,2), "geoPoint_07"},
                    {(1,3), "geoPoint_08"},
                    {(2,0), "geoPoint_09"},
                    {(2,1), "geoPoint_10"},
                    {(2,2), "geoPoint_11"},
                    {(2,3), "geoPoint_12"},
                    {(3,0), "geoPoint_13"},
                    {(3,1), "geoPoint_14"},
                    {(3,2), "geoPoint_15"},
                    {(3,3), "geoPoint_16"},
                });

            _serviceProvider = serviceCollection.BuildServiceProvider();
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
