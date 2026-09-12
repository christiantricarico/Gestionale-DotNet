using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.Email).HasMaxLength(255).IsRequired();
        builder.Property(e => e.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(e => e.FullName).HasMaxLength(255);
        builder.Property(e => e.Role).HasMaxLength(20).IsRequired();

        // Emails are normalised to lower case before being written, so a plain unique index
        // enforces uniqueness regardless of the database collation.
        builder.HasIndex(e => e.Email).IsUnique();

        builder.Ignore(e => e.IsLockedOut);
    }
}
