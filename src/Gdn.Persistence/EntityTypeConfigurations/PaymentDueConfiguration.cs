using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal sealed class PaymentDueConfiguration : IEntityTypeConfiguration<PaymentDue>
{
    public void Configure(EntityTypeBuilder<PaymentDue> builder)
    {
        // SQL Server does not allow OUTPUT clause on tables with AFTER triggers.
        // Disabling it forces EF Core to use a separate SELECT to retrieve the
        // generated identity after INSERT, which is compatible with our triggers.
        builder.ToTable(t => t.UseSqlOutputClause(false));

        // A Due cannot be deleted while it has payment allocations.
        builder.HasOne(e => e.Due)
               .WithMany(d => d.PaymentDues)
               .HasForeignKey(e => e.DueId)
               .OnDelete(DeleteBehavior.Restrict);

        // Deleting a Payment removes its allocations (handled via in-memory cascade +
        // TR_PaymentDues_AfterDelete trigger which decrements Due.PaidAmount).
        builder.HasOne(e => e.Payment)
               .WithMany(p => p.PaymentDues)
               .HasForeignKey(e => e.PaymentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.DueId, e.PaymentId });
    }
}
