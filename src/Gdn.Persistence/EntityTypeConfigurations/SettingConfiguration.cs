using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal sealed class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.Property(e => e.Key).HasMaxLength(100).IsRequired();

        // No length limit: the value holds the JSON payload of a whole settings section.
        builder.Property(e => e.Value).IsRequired();

        builder.HasIndex(e => e.Key).IsUnique();
    }
}
