using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Invoices;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class GetCreditNoteById
{
    public record GetCreditNoteByIdResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, string? MeasurementUnitCode, string? MeasurementUnitName,
        int? TaxRateId, string? TaxRateName, decimal? TaxRateValue);

    public record GetCreditNoteByIdResponseDue(int Id, DateOnly Date, decimal Amount, decimal PaidAmount, bool IsPaid);

    public record GetCreditNoteByIdResponse(int Id, string Number, DateOnly Date, int CustomerId,
        decimal? StampDutyAmount, bool StampDutyChargedToCustomer,
        string PaymentStatus,
        IEnumerable<GetCreditNoteByIdResponseRow> Rows,
        IEnumerable<GetCreditNoteByIdResponseDue> Dues);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/creditnotes/{id:int}", HandlerAsync).WithTags(Tags.CreditNotes);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, ICreditNoteRepository creditNoteRepository)
    {
        var data = await creditNoteRepository.GetAsync(id, ["Rows.TaxRate", "Rows.MeasurementUnit", "Dues"]);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound();
    }

    private static GetCreditNoteByIdResponse MapResponse(CreditNote creditNote)
        => new(creditNote.Id, creditNote.Number, creditNote.Date, creditNote.CustomerId,
               creditNote.StampDutyAmount, creditNote.StampDutyChargedToCustomer,
               ResolvePaymentStatus(creditNote),
               creditNote.Rows.Select(MapResponseRow),
               creditNote.Dues.Select(MapResponseDue));

    private static GetCreditNoteByIdResponseRow MapResponseRow(CreditNoteRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice,
               row.MeasurementUnitId, row.MeasurementUnit?.Code, row.MeasurementUnit?.Name,
               row.TaxRateId,
               string.IsNullOrWhiteSpace(row.TaxRate?.Name) ? row.TaxRate?.Code : row.TaxRate.Name,
               row.TaxRate?.Rate);

    private static GetCreditNoteByIdResponseDue MapResponseDue(Due due)
        => new(due.Id, due.Date, due.Amount, due.PaidAmount, due.IsPaid);

    private static string ResolvePaymentStatus(CreditNote creditNote)
    {
        if (!creditNote.Dues.Any())
            return PaymentStatus.NotPaid;

        if (creditNote.IsPaid)
            return PaymentStatus.Paid;

        return creditNote.Dues.Any(d => d.HasPayments)
            ? PaymentStatus.PartiallyPaid
            : PaymentStatus.NotPaid;
    }
}
