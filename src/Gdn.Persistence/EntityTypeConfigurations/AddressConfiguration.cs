using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(e => e.Street).HasMaxLength(255);
        builder.Property(e => e.PostalCode).HasMaxLength(10);
        builder.Property(e => e.City).HasMaxLength(50);
        builder.Property(e => e.Province).HasMaxLength(50);
        builder.Property(e => e.Country).HasMaxLength(50);
    }
}
