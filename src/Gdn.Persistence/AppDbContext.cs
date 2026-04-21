using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Gdn.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Address> Addresses { get; set; }
    public DbSet<CreditNote> CreditNotes { get; set; }
    public DbSet<CreditNoteRow> CreditNoteRows { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Due> Dues { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceRow> InvoiceRows { get; set; }
    public DbSet<Intervention> Interventions { get; set; }
    public DbSet<InterventionRow> InterventionRows { get; set; }
    public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentDue> PaymentDues { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Product> Products { get; set; }
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
