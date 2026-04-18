using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class GetInvoices
{
    public record GetInvoicesResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record GetInvoicesResponse(int Id, string Number, DateOnly Date, int CustomerId, string? CustomerName, string PaymentStatus, IEnumerable<GetInvoicesResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/invoices", HandlerAsync).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> HandlerAsync(IInvoiceRepository invoiceRepository)
    {
        var data = await invoiceRepository.GetAllAsync(["Customer", "Rows", "Dues"]);

        return ResultHelper.Ok(data.Select(MapResponse));
    }

    private static GetInvoicesResponse MapResponse(Invoice invoice)
        => new(invoice.Id, invoice.Number, invoice.Date, invoice.CustomerId, invoice.Customer.Name,
               ResolvePaymentStatus(invoice),
               invoice.Rows.Select(MapResponseRow));

    private static GetInvoicesResponseRow MapResponseRow(InvoiceRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);

    private static string ResolvePaymentStatus(Invoice invoice)
    {
        if (!invoice.Dues.Any())
            return PaymentStatus.NotPaid;

        if (invoice.IsPaid)
            return PaymentStatus.Paid;

        return invoice.Dues.Any(d => d.HasPayments)
            ? PaymentStatus.PartiallyPaid
            : PaymentStatus.NotPaid;
    }
}
