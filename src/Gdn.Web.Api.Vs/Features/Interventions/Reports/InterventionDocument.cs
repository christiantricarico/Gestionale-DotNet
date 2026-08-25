using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Gdn.Web.Api.Vs.Features.Interventions.Reports;

internal sealed class InterventionDocument(InterventionReportModel model) : IDocument
{
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(50);

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item()
                    .Text($"Rapporto di intervento #{model.Number}")
                    .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                column.Item().Text(text =>
                {
                    text.Span("Data rapporto: ").SemiBold();
                    text.Span($"{model.Date:d}");
                });
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(15);

            column.Item().Row(row =>
            {
                row.RelativeItem().Component(new AddressComponent("Fornitore", model.SellerAddress));
                row.ConstantItem(50);
                row.RelativeItem().Component(new AddressComponent("Cliente", model.CustomerAddress));
            });

            column.Item().Element(ComposeTable);
            column.Item().Element(ComposeSummary);
        });
    }

    private void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(25);
                columns.RelativeColumn(3);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("#");
                header.Cell().Element(CellStyle).Text("Descrizione");
                header.Cell().Element(CellStyle).AlignRight().Text("Prezzo unit.");
                header.Cell().Element(CellStyle).AlignRight().Text("Quantità");
                header.Cell().Element(CellStyle).AlignRight().Text("Totale");
                header.Cell().Element(CellStyle).AlignRight().Text("Iva");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                }
            });

            foreach (var item in model.Rows)
            {
                table.Cell().Element(CellStyle).Text($"{model.Rows.IndexOf(item) + 1}");
                table.Cell().Element(CellStyle).Text(item.Description);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice:c}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity:n3}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{(item.UnitPrice * item.Quantity):c}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.TaxRate:n0}%");

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            }
        });
    }

    private void ComposeSummary(IContainer container)
    {
        container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
        {
            column.Spacing(5);

            column.Item().Text("Riepilogo").FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Imponibile");
                row.RelativeItem().AlignRight().Text($"{CalculateNetAmount():c}").SemiBold();
            });

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Imposte");
                row.RelativeItem().AlignRight().Text($"{CalculateTaxAmount():c}").SemiBold();
            });

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Totale");
                row.RelativeItem().AlignRight().Text($"{CalculateTotalAmount():c}").SemiBold();
            });

            decimal CalculateNetAmount()
            {
                decimal netAmount = 0;
                foreach (var row in model.Rows)
                {
                    var rowNetAmount = Math.Round((row.UnitPrice * row.Quantity) ?? 0, 2, MidpointRounding.AwayFromZero);
                    netAmount += rowNetAmount;
                }

                return netAmount;
            }

            decimal CalculateTaxAmount()
            {
                decimal taxAmount = 0;

                var rowsGroupedByTaxRate = model.Rows
                    .Where(r => r.TaxRate.HasValue)
                    .GroupBy(x => x.TaxRate);

                foreach (var taxGroup in rowsGroupedByTaxRate)
                {
                    decimal groupNetAmount = taxGroup.Sum(x => Math.Round((x.UnitPrice * x.Quantity) ?? 0, 2, MidpointRounding.AwayFromZero));

                    decimal taxRate = taxGroup.Key ?? 0;
                    decimal groupTaxAmount = groupNetAmount * taxRate / 100;
                    taxAmount += Math.Round(groupTaxAmount, 2, MidpointRounding.AwayFromZero);
                }

                return taxAmount;
            }

            decimal CalculateTotalAmount()
            {
                return CalculateNetAmount() + CalculateTaxAmount();
            }
        });
    }
}

internal sealed class AddressComponent(string title, AddressModel address) : IComponent
{
    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);

            column.Item().BorderBottom(1).PaddingBottom(5).Text(title).SemiBold();

            column.Item().Text(address.CompanyName);
            column.Item().Text(address.Street);
            column.Item().Text($"{address.PostalCode} {address.City}, {address.Province}");
            column.Item().Text(address.Email);
            column.Item().Text(address.Phone);
        });
    }
}
