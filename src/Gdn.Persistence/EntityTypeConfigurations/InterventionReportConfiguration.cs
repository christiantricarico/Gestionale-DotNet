using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal sealed class InterventionReportConfiguration : IEntityTypeConfiguration<InterventionReport>
{
    public void Configure(EntityTypeBuilder<InterventionReport> builder)
    {
        builder.Property(e => e.Number).HasMaxLength(50);

        builder.HasIndex(e => e.CustomerId);
        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.IsInvoiced);
        builder.HasIndex(e => e.InvoiceId);

        builder.HasOne(e => e.Invoice)
            .WithMany(i => i.InterventionReports)
            .HasForeignKey(e => e.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class InterventionReportRowConfiguration : IEntityTypeConfiguration<InterventionReportRow>
{
    public void Configure(EntityTypeBuilder<InterventionReportRow> builder)
    {
        builder.Property(e => e.RowType).HasMaxLength(3);
    }
}
