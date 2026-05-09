using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal sealed class InterventionConfiguration : IEntityTypeConfiguration<Intervention>
{
    public void Configure(EntityTypeBuilder<Intervention> builder)
    {
        builder.Property(e => e.Number).HasMaxLength(50);

        builder.HasIndex(e => e.CustomerId);
        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.IsInvoiced);
        builder.HasIndex(e => e.InvoiceId);

        builder.HasOne(e => e.Invoice)
            .WithMany(i => i.Interventions)
            .HasForeignKey(e => e.InvoiceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

internal sealed class InterventionRowConfiguration : IEntityTypeConfiguration<InterventionRow>
{
    public void Configure(EntityTypeBuilder<InterventionRow> builder)
    {
        builder.Property(e => e.RowType).HasMaxLength(3);
        builder.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.SetNull);
    }
}
