﻿using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.Property(e => e.Code).HasMaxLength(10);
        builder.Property(e => e.Name).HasMaxLength(255);
    }
}
