using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Interventions;

public class GetInterventionById
{
    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, string? MeasurementUnitCode, string? MeasurementUnitName,
        int? TaxRateId, string? TaxRateName, decimal? TaxRateValue, int? ProductId);

    public record Response(int Id, string Number, DateOnly Date, int CustomerId, bool IsInvoiced, int? InvoiceId, string? InvoiceNumber, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/interventions/{id:int}", HandlerAsync).WithTags(Tags.Interventions);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IInterventionRepository reportRepository)
    {
        var data = await reportRepository.GetAsync(id, ["Rows.TaxRate", "Rows.MeasurementUnit", "Rows.Product", "Invoice"]);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound(InterventionErrors.NotFound(id));
    }

    private static Response MapResponse(Intervention report)
        => new(report.Id, report.Number, report.Date, report.CustomerId, report.IsInvoiced, report.InvoiceId, report.Invoice?.Number, report.Rows.Select(MapResponseRow));

    private static ResponseRow MapResponseRow(InterventionRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice,
               row.MeasurementUnitId, row.MeasurementUnit?.Code, row.MeasurementUnit?.Name,
               row.TaxRateId,
               string.IsNullOrWhiteSpace(row.TaxRate?.Name) ? row.TaxRate?.Code : row.TaxRate?.Name,
               row.TaxRate?.Rate, row.ProductId);
}
