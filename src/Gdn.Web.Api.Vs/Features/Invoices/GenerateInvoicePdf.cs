using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class GenerateInvoicePdf
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/invoices/pdf/{id}", Handler).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> Handler(IInvoiceRepository invoiceRepository, int id)
    {
        await GeneratePdfAsync(invoiceRepository, id);

        return ResultHelper.Ok("PDF content here");
    }

    private static async Task GeneratePdfAsync(IInvoiceRepository invoiceRepository, int invoiceId)
    {
        InvoiceReportModel model = await GetReportDataAsync(invoiceRepository, invoiceId);
        var document = new InvoiceDocument(model);
        document.GeneratePdfAndShow();
    }

    private static async Task<InvoiceReportModel> GetReportDataAsync(IInvoiceRepository invoiceRepository, int invoiceId)
    {
        Invoice? data = await invoiceRepository.GetAsync(invoiceId, ["Customer.Addresses", "Rows.TaxRate", "Rows.MeasurementUnit"]);

        if (data is null)
            throw new Exception("Invoice not found");

        var customer = data.Customer;
        var customerAddress = customer.Addresses.FirstOrDefault();

        var reportModel = new InvoiceReportModel
        {
            Number = data.Number,
            Date = data.Date,
            CustomerName = data.Customer?.Name,
            Notes = "Test di generazione report fattura con QuestPDF",
            SellerAddress = new Address()
            {
                CompanyName = "Seller Name",
                Street = "Seller Street",
                City = "Seller City",
                State = "Seller State",
                Email = "Seller Email",
                Phone = "Seller Phone"
            },
            CustomerAddress = new Address
            {
                CompanyName = customer?.Name,
                Street = customerAddress?.Street,
                City = customerAddress?.City,
                State = customerAddress?.Province,
                Email = customer?.Email,
                Phone = customer?.Phone
            },
            Rows = data.Rows.Select(row => new InvoiceRowReportModel
            {
                Description = row.Description,
                Quantity = row.Quantity,
                UnitPrice = row.UnitPrice,
                MeasurementUnitCode = row.MeasurementUnit?.Code,
                TaxRate = row.TaxRate?.Rate
            }).ToList()
        };

        return reportModel;
    }

    internal sealed class InvoiceReportModel
    {
        public string Number { get; set; } = default!;
        public DateOnly Date { get; set; }
        public string? CustomerName { get; set; }
        public string? Notes { get; set; }
        public Address SellerAddress { get; set; } = default!;
        public Address CustomerAddress { get; set; } = default!;
        public List<InvoiceRowReportModel> Rows { get; set; } = new();
    }

    internal sealed class InvoiceRowReportModel
    {
        public string? Description { get; set; }
        public string? MeasurementUnitCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TaxRate { get; set; }
    }

    internal sealed class Address
    {
        public string? CompanyName { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public object? Email { get; set; }
        public string? Phone { get; set; }
    }

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
                        text.Span("Issue date: ").SemiBold();
                        text.Span($"{Model.Date:d}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("Due date: ").SemiBold();
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
                    row.RelativeItem().Component(new AddressComponent("Da", Model.SellerAddress));
                    row.ConstantItem(50);
                    row.RelativeItem().Component(new AddressComponent("Per", Model.CustomerAddress));
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
                    header.Cell().Element(CellStyle).Text("Product");
                    header.Cell().Element(CellStyle).AlignRight().Text("Unit price");
                    header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                    header.Cell().Element(CellStyle).AlignRight().Text("Total");

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
        private Address Address { get; }

        public AddressComponent(string title, Address address)
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
                column.Item().Text($"{Address.City}, {Address.State}");
                column.Item().Text(Address.Email);
                column.Item().Text(Address.Phone);
            });
        }
    }
}