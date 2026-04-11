using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal class CreditNoteConfiguration : IEntityTypeConfiguration<CreditNote>
{
    public void Configure(EntityTypeBuilder<CreditNote> builder)
    {
        builder.Property(e => e.Number).HasMaxLength(50);
    }
}

internal class CreditNoteRowConfiguration : IEntityTypeConfiguration<CreditNoteRow>
{
    public void Configure(EntityTypeBuilder<CreditNoteRow> builder)
    {
        builder.Property(e => e.RowType).HasMaxLength(3);
    }
}
