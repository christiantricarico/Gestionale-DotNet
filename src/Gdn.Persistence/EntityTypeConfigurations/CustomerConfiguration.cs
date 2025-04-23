using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(e => e.Code).HasMaxLength(10);
        builder.Property(e => e.Name).HasMaxLength(255);
        builder.Property(e => e.FiscalCode).HasMaxLength(20);
        builder.Property(e => e.VatNumber).HasMaxLength(20);
        builder.Property(e => e.Phone).HasMaxLength(50);
        builder.Property(e => e.Email).HasMaxLength(255);
        builder.Property(e => e.Website).HasMaxLength(255);
        builder.Property(e => e.Pec).HasMaxLength(255);
        builder.Property(e => e.Sdi).HasMaxLength(10);
    }
}
