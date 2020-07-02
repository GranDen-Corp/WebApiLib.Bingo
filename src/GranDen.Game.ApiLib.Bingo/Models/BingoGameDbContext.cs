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
            modelBuilder.Entity<Bingo2dGameInfo>(g =>
            {
                g.HasIndex(p => p.GameName).IsUnique();
                g.Property(b => b.MaxWidth);
                g.Property(b => b.MaxHeight);
                g.HasMany(p => p.JoinedPlayers);
                g.HasMany(e => e.BingoPoints);
            });

            modelBuilder.Entity<BingoPlayerInfo>(p =>
            {
                p.HasMany(e => e.JoinedGames);
            });

            modelBuilder.Entity<BingoPoint>(b =>
            {
                b.HasOne(p => p.PointProjection)
                    .WithOne(p => p.BingoPoint)
                    .HasForeignKey<PointProjection>(p => p.BingoPointFk);

                b.HasOne(e => e.BelongingGame);

                b.HasOne(e => e.BelongingPlayer);

                b.OwnsOne(x => x.MarkPoint, m =>
                {
                    m.Property(p => p.X).IsRequired().HasColumnName(nameof(MarkPoint2D.X));
                    m.Property(p => p.Y).IsRequired().HasColumnName(nameof(MarkPoint2D.Y));
                    m.Property(p => p.Marked).HasColumnName(nameof(MarkPoint2D.Marked));
                });
            });

            modelBuilder.Entity<PointProjection>().HasOne(p => p.MappingGeoPoint);

            modelBuilder.Entity<MappingGeoPoint>().HasIndex(m => m.GeoPointId).IsUnique();
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
