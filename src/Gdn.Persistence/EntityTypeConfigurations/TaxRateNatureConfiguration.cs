using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdn.Persistence.EntityTypeConfigurations;

internal class TaxRateNatureConfiguration : IEntityTypeConfiguration<TaxRateNature>
{
    public void Configure(EntityTypeBuilder<TaxRateNature> builder)
    {
        builder.Property(e => e.Code).HasMaxLength(10);
        builder.Property(e => e.Name).HasMaxLength(255);

        builder.HasData(
            new() { Id = 1, Code = "N1", Name = "N1 : Escluso art.15" },
            new() { Id = 2, Code = "N2", Name = "N2 : Non soggette" },
            new() { Id = 3, Code = "N2.1", Name = "N2.1 : Non soggette artt. Da 7 a 7-septies" },
            new() { Id = 4, Code = "N2.2", Name = "N2.2 : Non soggette - altri casi" },
            new() { Id = 5, Code = "N3", Name = "N3 : Non imponibili" },
            new() { Id = 6, Code = "N3.1", Name = "N3.1 : Non imponibili - esportazioni" },
            new() { Id = 7, Code = "N3.2", Name = "N3.2 : Non imponibili - cessioni intracomunitarie" },
            new() { Id = 8, Code = "N3.3", Name = "N3.3 : Non imponibili - cessioni verso San Marino" },
            new() { Id = 9, Code = "N3.4", Name = "N3.4 : Non imponibili - op. assimilate alle esportazioni" },
            new() { Id = 10, Code = "N3.5", Name = "N3.5 : Non imponibili - a seguito di dichiarazioni d?intento" },
            new() { Id = 11, Code = "N3.6", Name = "N3.6 : Non imponibili - altre op. no plafond" },
            new() { Id = 12, Code = "N4", Name = "N4 : Esenti" },
            new() { Id = 13, Code = "N5", Name = "N5 : Regime del margine / IVA non esposta" },
            new() { Id = 14, Code = "N6", Name = "N6 : Inversione contabile" },
            new() { Id = 15, Code = "N6.1", Name = "N6.1 : Inversione contabile - cessione di rottami" },
            new() { Id = 16, Code = "N6.2", Name = "N6.2 : Inversione contabile - cessione di oro e argento puro" },
            new() { Id = 17, Code = "N6.3", Name = "N6.3 : Inversione contabile - subappalto nel settore edile" },
            new() { Id = 18, Code = "N6.4", Name = "N6.4 : Inversione contabile - cessione di fabbricati" },
            new() { Id = 19, Code = "N6.5", Name = "N6.5 : Inversione contabile - cessione di telefoni cellulari" },
            new() { Id = 20, Code = "N6.6", Name = "N6.6 : Inversione contabile - cessione di prodotti elettronici" },
            new() { Id = 21, Code = "N6.7", Name = "N6.7 : Inversione contabile - prestazioni comparto edile" },
            new() { Id = 22, Code = "N6.8", Name = "N6.8 : Inversione contabile - op. settore energetico" },
            new() { Id = 23, Code = "N6.9", Name = "N6.9 : Inversione contabile - altri casi" },
            new() { Id = 24, Code = "N7", Name = "N7 : IVA assolta in altro stato UE" }
        );
    }
}
