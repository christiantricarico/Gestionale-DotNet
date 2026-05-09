using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(e => e.Code).HasMaxLength(50);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.Type).HasMaxLength(3).IsRequired();

        builder.HasIndex(e => e.Code);

        builder.HasOne(e => e.MeasurementUnit).WithMany().HasForeignKey(e => e.MeasurementUnitId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.TaxRate).WithMany().HasForeignKey(e => e.TaxRateId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.ProductCategory).WithMany().HasForeignKey(e => e.ProductCategoryId).OnDelete(DeleteBehavior.SetNull);
    }
}
