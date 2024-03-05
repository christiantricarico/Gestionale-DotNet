using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Gdn.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<TaxRate> TaxRates { get; set; }
    public DbSet<TaxRateNature> TaxRateNatures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //Default precision for decimal properties.
        configurationBuilder.Properties<decimal>()
            .HavePrecision(18, 6);
    }
}
