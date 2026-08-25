using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal sealed class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.Property(e => e.Number).HasMaxLength(50);

        builder.HasIndex(e => e.CustomerId);
        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.IsAccepted);
    }
}

internal sealed class QuoteRowConfiguration : IEntityTypeConfiguration<QuoteRow>
{
    public void Configure(EntityTypeBuilder<QuoteRow> builder)
    {
        builder.Property(e => e.RowType).HasMaxLength(3);
        builder.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.SetNull);
    }
}
