using Gdn.Domain.Data.Repositories;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;

namespace Gdn.Web.Api.Vs.Features.CreditNotes.Reports;

public class CreditNoteReportGenerator(IOptions<AppSettings> appSettings, ICreditNoteRepository creditNoteRepository)
{
    public async Task<byte[]> GeneratePdfBytesAsync(int creditNoteId)
    {
        CreditNoteReportModel model = await GetReportDataAsync(creditNoteId);
        var document = new CreditNoteDocument(model);
        var pdfBytes = document.GeneratePdf();
        return pdfBytes;
    }

    private async Task<CreditNoteReportModel> GetReportDataAsync(int creditNoteId)
    {
        var creditNote = await creditNoteRepository.GetAsync(creditNoteId, ["Customer.Addresses", "Rows.TaxRate", "Rows.MeasurementUnit", "Dues"])
            ?? throw new InvalidOperationException("Credit note not found");

        var company = appSettings.Value.CompanyData;
        var customer = creditNote.Customer;
        var customerAddress = customer.Addresses.FirstOrDefault();

        var reportModel = new CreditNoteReportModel
        {
            Number = creditNote.Number,
            Date = creditNote.Date,
            CustomerName = creditNote.Customer?.Name,
            StampDutyAmount = creditNote.StampDutyAmount,
            StampDutyChargedToCustomer = creditNote.StampDutyChargedToCustomer,
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
            Rows = creditNote.Rows.Select(row => new CreditNoteRowReportModel
            {
                Description = row.Description,
                Quantity = row.Quantity,
                UnitPrice = row.UnitPrice,
                MeasurementUnitCode = row.MeasurementUnit?.Code,
                TaxRate = row.TaxRate?.Rate
            }).ToList(),
            Dues = creditNote.Dues
                .OrderBy(d => d.Date)
                .Select(d => new DueReportModel
                {
                    Date = d.Date,
                    Amount = d.Amount
                }).ToList()
        };

        return reportModel;
    }
}
