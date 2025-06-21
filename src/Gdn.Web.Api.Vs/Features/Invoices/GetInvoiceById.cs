using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class GetInvoiceById
{
    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, string? MeasurementUnitCode, string? MeasurementUnitName,
        int? TaxRateId, string? TaxRateName);
    public record Response(int Id, string Number, DateOnly Date, int CustomerId, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/invoices/{id}", Handler).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> Handler(IInvoiceRepository invoiceRepository, int id)
    {
        var data = await invoiceRepository.GetAsync(id, ["Rows.TaxRate", "Rows.MeasurementUnit"]);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound();
    }

    private static Response MapResponse(Invoice invoice)
        => new(invoice.Id, invoice.Number, invoice.Date, invoice.CustomerId, invoice.Rows.Select(r => MapResponseRow(r)));

    private static ResponseRow MapResponseRow(InvoiceRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice,
            row.MeasurementUnitId, row.MeasurementUnit?.Code, row.MeasurementUnit?.Name,
            row.TaxRateId,
            string.IsNullOrWhiteSpace(row.TaxRate?.Name) ? row.TaxRate?.Code : row.TaxRate.Name);
}
