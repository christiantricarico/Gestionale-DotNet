using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Gdn.Web.Api.Vs.Features.Invoices.Reports;

internal sealed class InvoiceDocument : IDocument
{
    public InvoiceReportModel Model { get; }

    public InvoiceDocument(InvoiceReportModel model)
    {
        Model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
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
                    .Text($"Fattura #{Model.Number}")
                    .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                column.Item().Text(text =>
                {
                    text.Span("Data fattura: ").SemiBold();
                    text.Span($"{Model.Date:d}");
                });

                column.Item().Text(text =>
                {
                    text.Span("Data scadenza: ").SemiBold();
                    text.Span($"{Model.Date:d}");
                });
            });

            row.ConstantItem(100).Height(50).Placeholder();
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(5);

            column.Item().Row(row =>
            {
                row.RelativeItem().Component(new AddressComponent("Fornitore", Model.SellerAddress));
                row.ConstantItem(50);
                row.RelativeItem().Component(new AddressComponent("Cliente", Model.CustomerAddress));
            });

            column.Item().Element(ComposeTable);

            if (!string.IsNullOrWhiteSpace(Model.Notes))
                column.Item().PaddingTop(25).Element(ComposeComments);
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
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("#");
                header.Cell().Element(CellStyle).Text("Descrizione");
                header.Cell().Element(CellStyle).AlignRight().Text("Prezzo unit.");
                header.Cell().Element(CellStyle).AlignRight().Text("Quantità");
                header.Cell().Element(CellStyle).AlignRight().Text("Totale");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                }
            });

            foreach (var item in Model.Rows)
            {
                table.Cell().Element(CellStyle).Text($"{Model.Rows.IndexOf(item) + 1}");
                table.Cell().Element(CellStyle).Text(item.Description);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice:c}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity:n3}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{(item.UnitPrice * item.Quantity):c}");

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            }
        });
    }

    private void ComposeComments(IContainer container)
    {
        container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
        {
            column.Spacing(5);
            column.Item().Text("Note").FontSize(14);
            column.Item().Text(Model.Notes);
        });
    }
}

internal sealed class AddressComponent : IComponent
{
    private string Title { get; }
    private AddressModel Address { get; }

    public AddressComponent(string title, AddressModel address)
    {
        Title = title;
        Address = address;
    }

    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);

            column.Item().BorderBottom(1).PaddingBottom(5).Text(Title).SemiBold();

            column.Item().Text(Address.CompanyName);
            column.Item().Text(Address.Street);
            column.Item().Text($"{Address.PostalCode} {Address.City}, {Address.Province}");
            column.Item().Text(Address.Email);
            column.Item().Text(Address.Phone);
        });
    }
}