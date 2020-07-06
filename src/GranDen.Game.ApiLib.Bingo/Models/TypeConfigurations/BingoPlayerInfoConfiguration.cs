using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GranDen.Game.ApiLib.Bingo.Models.TypeConfigurations
{
    internal class BingoPlayerInfoConfiguration : IEntityTypeConfiguration<BingoPlayerInfo>
    {
        public void Configure(EntityTypeBuilder<BingoPlayerInfo> builder)
        {
            builder.HasMany(e => e.JoinedGames);
        }
    }
}
