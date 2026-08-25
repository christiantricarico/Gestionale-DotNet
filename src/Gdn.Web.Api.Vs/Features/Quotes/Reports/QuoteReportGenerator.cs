using Gdn.Domain.Data.Repositories;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;

namespace Gdn.Web.Api.Vs.Features.Quotes.Reports;

public class QuoteReportGenerator(IOptions<AppSettings> appSettings, IQuoteRepository quoteRepository)
{
    public async Task<byte[]?> GeneratePdfBytesAsync(int quoteId)
    {
        var model = await GetReportDataAsync(quoteId);
        if (model is null)
            return null;

        var document = new QuoteDocument(model);
        var pdfBytes = document.GeneratePdf();
        return pdfBytes;
    }

    private async Task<QuoteReportModel?> GetReportDataAsync(int quoteId)
    {
        var quote = await quoteRepository.GetAsync(quoteId, ["Customer.Addresses", "Rows.TaxRate", "Rows.MeasurementUnit"]);
        if (quote is null)
            return null;

        var company = appSettings.Value.CompanyData;
        var customer = quote.Customer;
        var customerAddress = customer.Addresses.FirstOrDefault();

        var reportModel = new QuoteReportModel
        {
            Number = quote.Number,
            Date = quote.Date,
            CustomerName = quote.Customer?.Name,
            AcceptanceStatusLabel = quote.IsAccepted
                ? $"ACCETTATO il {quote.AcceptedAt:dd/MM/yyyy}"
                : "NON ACCETTATO",
            SellerAddress = new AddressModel
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
            Rows = quote.Rows.Select(row => new QuoteRowReportModel
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
