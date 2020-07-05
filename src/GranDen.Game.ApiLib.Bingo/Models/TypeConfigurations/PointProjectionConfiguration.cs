using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GranDen.Game.ApiLib.Bingo.Models.TypeConfigurations
{
    internal class PointProjectionConfiguration : IEntityTypeConfiguration<PointProjection>
    {
        public void Configure(EntityTypeBuilder<PointProjection> builder)
        {
           builder.HasOne(p => p.MappingGeoPoint); 
        }
    }
}
