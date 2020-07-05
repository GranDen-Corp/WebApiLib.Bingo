using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GranDen.Game.ApiLib.Bingo.Models.TypeConfigurations
{
    internal class Bingo2dGameInfoConfiguration : IEntityTypeConfiguration<Bingo2dGameInfo>
    {
        public void Configure(EntityTypeBuilder<Bingo2dGameInfo> builder)
        {
            builder.HasIndex(p => p.GameName).IsUnique();
            builder.Property(b => b.MaxWidth);
            builder.Property(b => b.MaxHeight);
            builder.HasMany(p => p.JoinedPlayers);
            builder.HasMany(e => e.BingoPoints);
        }
    }
}
