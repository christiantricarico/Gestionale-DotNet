using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasOne(e => e.PaymentMethod)
               .WithMany()
               .HasForeignKey(e => e.PaymentMethodId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Customer)
               .WithMany()
               .HasForeignKey(e => e.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.CustomerId);
    }
}
