using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;

namespace Gdn.Web.Api.Vs.Features.Invoices.Reports;

public class InvoiceReportGenerator(IOptions<CompanyData> companyData, IInvoiceRepository invoiceRepository)
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
        Invoice? data = await invoiceRepository.GetAsync(invoiceId, ["Customer.Addresses", "Rows.TaxRate", "Rows.MeasurementUnit"]);

        if (data is null)
            throw new Exception("Invoice not found");

        var company = companyData.Value;
        var customer = data.Customer;
        var customerAddress = customer.Addresses.FirstOrDefault();

        var reportModel = new InvoiceReportModel
        {
            Number = data.Number,
            Date = data.Date,
            CustomerName = data.Customer?.Name,
            Notes = "Test di generazione report fattura con QuestPDF",
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
}
