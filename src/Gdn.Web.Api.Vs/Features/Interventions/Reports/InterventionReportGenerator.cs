using Gdn.Domain.Data.Repositories;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;

namespace Gdn.Web.Api.Vs.Features.Interventions.Reports;

public class InterventionReportGenerator(IOptions<AppSettings> appSettings, IInterventionRepository reportRepository)
{
    public async Task<byte[]> GeneratePdfBytesAsync(int reportId)
    {
        InterventionReportModel model = await GetReportDataAsync(reportId);
        var document = new InterventionDocument(model);
        var pdfBytes = document.GeneratePdf();
        return pdfBytes;
    }

    private async Task<InterventionReportModel> GetReportDataAsync(int reportId)
    {
        var report = await reportRepository.GetAsync(reportId, ["Customer.Addresses", "Rows.TaxRate", "Rows.MeasurementUnit"])
            ?? throw new InvalidOperationException("Intervention report not found");

        var company = appSettings.Value.CompanyData;
        var customer = report.Customer;
        var customerAddress = customer.Addresses.FirstOrDefault();

        var reportModel = new InterventionReportModel
        {
            Number = report.Number,
            Date = report.Date,
            CustomerName = report.Customer?.Name,
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
            Rows = report.Rows.Select(row => new InterventionRowReportModel
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
