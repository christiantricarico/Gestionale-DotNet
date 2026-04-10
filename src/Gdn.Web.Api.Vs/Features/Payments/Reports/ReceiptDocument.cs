using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Gdn.Web.Api.Vs.Features.Payments.Reports;

internal sealed class ReceiptDocument(ReceiptReportModel model) : IDocument
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
                    .Text($"Distinta di incasso #{model.PaymentId}")
                    .FontSize(20).SemiBold().FontColor(Colors.Green.Medium);

                column.Item().Text(text =>
                {
                    text.Span("Data incasso: ").SemiBold();
                    text.Span($"{model.Date:d}");
                });

                column.Item().Text(text =>
                {
                    text.Span("Importo totale: ").SemiBold();
                    text.Span($"{model.Amount:C2}");
                });

                if (!string.IsNullOrWhiteSpace(model.PaymentMethodName))
                {
                    column.Item().Text(text =>
                    {
                        text.Span("Metodo di pagamento: ").SemiBold();
                        text.Span(model.PaymentMethodName);
                    });
                }
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
                row.RelativeItem().Component(new AddressComponent("Emittente", model.SellerAddress));
                row.ConstantItem(50);
                row.RelativeItem().Component(new AddressComponent("Cliente", model.CustomerAddress));
            });

            column.Item().Element(ComposeDuesTable);
            column.Item().Element(ComposeTotals);
        });
    }

    private void ComposeDuesTable(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(5);

            column.Item().Text("Scadenze coperte dall'incasso")
                .FontSize(14).SemiBold().FontColor(Colors.Green.Medium);

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("N. Documento");
                    header.Cell().Element(HeaderCellStyle).Text("Tipo");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Data scadenza");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Importo scadenza");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Importo coperto");

                    static IContainer HeaderCellStyle(IContainer c) =>
                        c.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                });

                foreach (var due in model.CoveredDues)
                {
                    table.Cell().Element(RowCellStyle).Text(due.DocumentNumber ?? "—");
                    table.Cell().Element(RowCellStyle).Text(due.DocumentType);
                    table.Cell().Element(RowCellStyle).AlignRight().Text($"{due.DueDate:d}");
                    table.Cell().Element(RowCellStyle).AlignRight().Text($"{due.DueAmount:C2}");
                    table.Cell().Element(RowCellStyle).AlignRight().Text($"{due.AllocatedAmount:C2}");

                    static IContainer RowCellStyle(IContainer c) =>
                        c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            });
        });
    }

    private void ComposeTotals(IContainer container)
    {
        container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
        {
            column.Spacing(5);

            column.Item().Text("Riepilogo").FontSize(14).SemiBold().FontColor(Colors.Green.Medium);

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Totale allocato");
                row.RelativeItem().AlignRight()
                    .Text($"{model.CoveredDues.Sum(d => d.AllocatedAmount):C2}").SemiBold();
            });

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Importo incasso");
                row.RelativeItem().AlignRight()
                    .Text($"{model.Amount:C2}").SemiBold();
            });
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

            if (!string.IsNullOrWhiteSpace(address.CompanyName))
                column.Item().Text(address.CompanyName);
            if (!string.IsNullOrWhiteSpace(address.Street))
                column.Item().Text(address.Street);
            if (!string.IsNullOrWhiteSpace(address.City))
                column.Item().Text($"{address.PostalCode} {address.City}, {address.Province}".Trim(' ', ','));
            if (!string.IsNullOrWhiteSpace(address.Email))
                column.Item().Text(address.Email);
            if (!string.IsNullOrWhiteSpace(address.Phone))
                column.Item().Text(address.Phone);
        });
    }
}
