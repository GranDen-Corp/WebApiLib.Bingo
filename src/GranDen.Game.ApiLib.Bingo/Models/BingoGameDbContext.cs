using System.Reflection;
using GranDen.Game.ApiLib.Bingo.Models.TypeConfigurations;
using GranDen.GameLib.Bingo;
using Microsoft.EntityFrameworkCore;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    /// <summary>
    /// Bingo Game database context
    /// </summary>
    public class BingoGameDbContext : DbContext
    {
        /// <inheritdoc />
        public BingoGameDbContext(DbContextOptions<BingoGameDbContext> options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Bingo2dGameInfoConfiguration());
            modelBuilder.ApplyConfiguration(new BingoPlayerInfoConfiguration());            
            modelBuilder.ApplyConfiguration(new BingoPointConfiguration());
            modelBuilder.ApplyConfiguration(new PointProjectionConfiguration());
            modelBuilder.ApplyConfiguration(new MappingGeoPointConfiguration());
        }

        /// <summary>
        /// <c>Bingo2dGameInfo</c> DB set
        /// </summary>
        public DbSet<Bingo2dGameInfo> Bingo2dGameInfos { get; set; }
        
        /// <summary>
        /// <c>BingoPlayerInfo</c> DB set
        /// </summary>
        public DbSet<BingoPlayerInfo> BingoPlayerInfos { get; set; }
        
        /// <summary>
        /// <c>BingoPoint</c> DB set
        /// </summary>
        public DbSet<BingoPoint> BingoPoints { get; set; }
        
        /// <summary>
        /// <c>MappingGeoPoint</c> DB set
        /// </summary>
        public DbSet<MappingGeoPoint> MappingGeoPoints { get; set; }
    }
}
