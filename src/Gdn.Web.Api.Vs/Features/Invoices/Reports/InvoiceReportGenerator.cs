using Gdn.Domain.Data.Repositories;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;

namespace Gdn.Web.Api.Vs.Features.Invoices.Reports;

public class InvoiceReportGenerator(IOptions<AppSettings> appSettings, IInvoiceRepository invoiceRepository)
{
    public async Task<byte[]> GeneratePdfBytesAsync(int invoiceId)
    {
        InvoiceReportModel model = await GetReportDataAsync(invoiceId);
        var document = new InvoiceDocument(model);
        var pdfBytes = document.GeneratePdf();
        return pdfBytes;
    }

    private async Task<InvoiceReportModel> GetReportDataAsync(int invoiceId)
    {
        var invoice = await invoiceRepository.GetAsync(invoiceId, ["Customer.Addresses", "Rows.TaxRate", "Rows.MeasurementUnit"])
            ?? throw new InvalidOperationException("Invoice not found");

        var company = appSettings.Value.CompanyData;
        var customer = invoice.Customer;
        var customerAddress = customer.Addresses.FirstOrDefault();

        var reportModel = new InvoiceReportModel
        {
            Number = invoice.Number,
            Date = invoice.Date,
            CustomerName = invoice.Customer?.Name,
            Notes = "Test di generazione report fattura con QuestPDF",
            StampDutyAmount = invoice.StampDutyAmount,
            StampDutyChargedToCustomer = invoice.StampDutyChargedToCustomer,
            SellerAddress = new AddressModel()
            {
                CompanyName = company.Name,
                Street = company.Street,
                PostalCode = company.PostalCode,
                City = company.City,
                Province = company.Province,
                Email = company.Email,
                Phone = company.Phone
            },
            CustomerAddress = new AddressModel
            {
                CompanyName = customer?.Name,
                Street = customerAddress?.Street,
                PostalCode = customerAddress?.PostalCode,
                City = customerAddress?.City,
                Province = customerAddress?.Province,
                Email = customer?.Email,
                Phone = customer?.Phone
            },
            Rows = invoice.Rows.Select(row => new InvoiceRowReportModel
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
}
