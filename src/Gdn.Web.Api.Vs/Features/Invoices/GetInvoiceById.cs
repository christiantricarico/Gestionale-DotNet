using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class GetInvoiceById
{
    public record GetInvoiceByIdResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, string? MeasurementUnitCode, string? MeasurementUnitName,
        int? TaxRateId, string? TaxRateName, decimal? TaxRateValue, int? ProductId);

    public record GetInvoiceByIdResponseDue(int Id, DateOnly Date, decimal Amount, decimal PaidAmount, bool IsPaid);

    public record GetInvoiceByIdResponse(int Id, string Number, DateOnly Date, int CustomerId,
        decimal? StampDutyAmount, bool StampDutyChargedToCustomer,
        string PaymentStatus,
        IEnumerable<GetInvoiceByIdResponseRow> Rows,
        IEnumerable<GetInvoiceByIdResponseDue> Dues,
        IEnumerable<int> InterventionIds);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/invoices/{id:int}", HandlerAsync).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IInvoiceRepository invoiceRepository)
    {
        var data = await invoiceRepository.GetAsync(id, ["Rows.TaxRate", "Rows.MeasurementUnit", "Rows.Product", "Dues", "Interventions"]);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound();
    }

    private static GetInvoiceByIdResponse MapResponse(Invoice invoice)
        => new(invoice.Id, invoice.Number, invoice.Date, invoice.CustomerId,
                invoice.StampDutyAmount, invoice.StampDutyChargedToCustomer,
                ResolvePaymentStatus(invoice),
                invoice.Rows.Select(MapResponseRow),
                invoice.Dues.Select(MapResponseDue),
                invoice.Interventions.Select(ir => ir.Id));

    private static GetInvoiceByIdResponseRow MapResponseRow(InvoiceRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice,
               row.MeasurementUnitId, row.MeasurementUnit?.Code, row.MeasurementUnit?.Name,
               row.TaxRateId,
               string.IsNullOrWhiteSpace(row.TaxRate?.Name) ? row.TaxRate?.Code : row.TaxRate.Name,
               row.TaxRate?.Rate, row.ProductId);

    private static GetInvoiceByIdResponseDue MapResponseDue(Due due)
        => new(due.Id, due.Date, due.Amount, due.PaidAmount, due.IsPaid);

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
