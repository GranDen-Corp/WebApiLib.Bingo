using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GranDen.Game.ApiLib.Bingo.Models.TypeConfigurations
{
    internal class MappingGeoPointConfiguration : IEntityTypeConfiguration<MappingGeoPoint>
    {
        public void Configure(EntityTypeBuilder<MappingGeoPoint> builder)
        {
            builder.HasIndex(m => m.GeoPointId).IsUnique();
        }
    }
}
