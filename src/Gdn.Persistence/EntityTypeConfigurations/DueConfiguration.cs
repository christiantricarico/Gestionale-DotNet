using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal sealed class DueConfiguration : IEntityTypeConfiguration<Due>
{
    public void Configure(EntityTypeBuilder<Due> builder)
    {
        builder.HasIndex(e => e.InvoiceId);
        builder.HasIndex(e => e.CreditNoteId);

        // PaidAmount is managed exclusively by a SQL trigger; the application never writes it.
        builder.Property(e => e.PaidAmount).HasDefaultValue(0m);

        builder.HasOne(e => e.Invoice)
               .WithMany(i => i.Dues)
               .HasForeignKey(e => e.InvoiceId)
               .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(e => e.CreditNote)
               .WithMany(cn => cn.Dues)
               .HasForeignKey(e => e.CreditNoteId)
               .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(e => e.Customer)
               .WithMany()
               .HasForeignKey(e => e.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
