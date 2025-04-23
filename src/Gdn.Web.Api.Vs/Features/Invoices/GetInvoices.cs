using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class GetInvoices
{
    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record Response(int Id, string Number, DateOnly Date, int CustomerId, string? CustomerName, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/invoices", Handler).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> Handler(IInvoiceRepository invoiceRepository)
    {
        var data = await invoiceRepository.GetAllAsync(["Customer", "Rows"]);
        var responseData = data.Select(e => MapResponse(e));

        return ResultHelper.Ok(responseData);
    }

    private static Response MapResponse(Invoice invoice)
        => new(invoice.Id, invoice.Number, invoice.Date, invoice.CustomerId, invoice.Customer.Name, invoice.Rows.Select(r => MapResponseRow(r)));

    private static ResponseRow MapResponseRow(InvoiceRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);
}
