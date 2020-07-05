using GranDen.GameLib.Bingo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GranDen.Game.ApiLib.Bingo.Models.TypeConfigurations
{
    internal class BingoPointConfiguration : IEntityTypeConfiguration<BingoPoint>
    {
        public void Configure(EntityTypeBuilder<BingoPoint> builder)
        {
            builder.HasOne(p => p.PointProjection)
                .WithOne(p => p.BingoPoint)
                .HasForeignKey<PointProjection>(p => p.BingoPointFk);

            builder.HasOne(e => e.BelongingGame);

            builder.HasOne(e => e.BelongingPlayer);

            builder.OwnsOne(x => x.MarkPoint, m =>
            {
                m.Property(p => p.X).IsRequired().HasColumnName(nameof(MarkPoint2D.X));
                m.Property(p => p.Y).IsRequired().HasColumnName(nameof(MarkPoint2D.Y));
                m.Property(p => p.Marked).HasColumnName(nameof(MarkPoint2D.Marked));
            });
        }
    }
}
