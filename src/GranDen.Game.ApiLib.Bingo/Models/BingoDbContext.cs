using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace GranDen.Game.ApiLib.Bingo.Models
{
    public class BingoDbContext : DbContext
    {
        public BingoDbContext(DbContextOptions<BingoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BingoGameInfo>(g =>
            {
                g.HasMany(p => p.JoinedPlayers);
            });

            modelBuilder.Entity<BingoPlayerInfo>(p =>
            {
                p.HasMany(e => e.BingoPoints);
                p.HasOne(e => e.BingoGame);
            });

            modelBuilder.Entity<BingoPoint>(b =>
            {
                b.HasOne(p => p.PointProjection)
                    .WithOne(p => p.BingoPoint)
                    .HasForeignKey<PointProjection>(p => p.BingoPointFk);

                b.Property(e => e.MarkPoint).HasConversion(MarkPoint2DValueEfCoreUtil.GetMarkPoint2DValueConverter());
            });

            modelBuilder.Entity<PointProjection>().HasOne(p => p.MappingGeoPoint);

            modelBuilder.Entity<MappingGeoPoint>().HasIndex(m => m.GeoPointId).IsUnique();
        }

        public DbSet<BingoGameInfo> BingoGameInfos { get; set; }
        public DbSet<BingoPlayerInfo> BingoPlayerInfos { get; set; }
        public DbSet<BingoPoint> BingoPoints { get; set; }
        public DbSet<MappingGeoPoint> MappingGeoPoints { get; set; }
    }
}