using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class GetInvoiceById
{
    public record GetInvoiceByIdResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, string? MeasurementUnitCode, string? MeasurementUnitName,
        int? TaxRateId, string? TaxRateName, decimal? TaxRateValue);
    public record GetInvoiceByIdResponse(int Id, string Number, DateOnly Date, int CustomerId, decimal? StampDutyAmount, bool StampDutyChargedToCustomer, IEnumerable<GetInvoiceByIdResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/invoices/{id:int}", Handler).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> Handler(int id, IInvoiceRepository invoiceRepository)
    {
        var data = await invoiceRepository.GetAsync(id, ["Rows.TaxRate", "Rows.MeasurementUnit"]);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound();
    }

    private static GetInvoiceByIdResponse MapResponse(Invoice invoice)
        => new(invoice.Id, invoice.Number, invoice.Date, invoice.CustomerId, invoice.StampDutyAmount, invoice.StampDutyChargedToCustomer, invoice.Rows.Select(r => MapResponseRow(r)));

    private static GetInvoiceByIdResponseRow MapResponseRow(InvoiceRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice,
            row.MeasurementUnitId, row.MeasurementUnit?.Code, row.MeasurementUnit?.Name,
            row.TaxRateId,
            string.IsNullOrWhiteSpace(row.TaxRate?.Name) ? row.TaxRate?.Code : row.TaxRate.Name,
            row.TaxRate?.Rate);
}
