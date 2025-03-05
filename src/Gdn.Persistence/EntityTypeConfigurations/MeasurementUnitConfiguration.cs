using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal class MeasurementUnitConfiguration : IEntityTypeConfiguration<MeasurementUnit>
{
    public void Configure(EntityTypeBuilder<MeasurementUnit> builder)
    {
        builder.Property(e => e.Code).HasMaxLength(10);
        builder.Property(e => e.Name).HasMaxLength(255);
    }
}
