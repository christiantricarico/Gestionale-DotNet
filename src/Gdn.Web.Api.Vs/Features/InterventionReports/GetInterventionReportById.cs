using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.InterventionReports;

public class GetInterventionReportById
{
    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, string? MeasurementUnitCode, string? MeasurementUnitName,
        int? TaxRateId, string? TaxRateName, decimal? TaxRateValue);

    public record Response(int Id, string Number, DateOnly Date, int CustomerId, bool IsInvoiced, int? InvoiceId, string? InvoiceNumber, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/interventionreports/{id:int}", HandlerAsync).WithTags(Tags.InterventionReports);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IInterventionReportRepository reportRepository)
    {
        var data = await reportRepository.GetAsync(id, ["Rows.TaxRate", "Rows.MeasurementUnit", "Invoice"]);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound(InterventionReportErrors.NotFound(id));
    }

    private static Response MapResponse(InterventionReport report)
        => new(report.Id, report.Number, report.Date, report.CustomerId, report.IsInvoiced, report.InvoiceId, report.Invoice?.Number, report.Rows.Select(MapResponseRow));

    private static ResponseRow MapResponseRow(InterventionReportRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice,
               row.MeasurementUnitId, row.MeasurementUnit?.Code, row.MeasurementUnit?.Name,
               row.TaxRateId,
               string.IsNullOrWhiteSpace(row.TaxRate?.Name) ? row.TaxRate?.Code : row.TaxRate?.Name,
               row.TaxRate?.Rate);
}
