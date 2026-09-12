using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Invoices;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class GetCreditNotes
{
    public record GetCreditNotesResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record GetCreditNotesResponse(int Id, string Number, DateOnly Date, int CustomerId, string? CustomerName, string PaymentStatus, IEnumerable<GetCreditNotesResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/creditnotes", HandlerAsync).WithTags(Tags.CreditNotes).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(ICreditNoteRepository creditNoteRepository)
    {
        var data = await creditNoteRepository.GetAllAsync(["Customer", "Rows", "Dues"]);

        return ResultHelper.Ok(data.Select(MapResponse));
    }

    private static GetCreditNotesResponse MapResponse(CreditNote creditNote)
        => new(creditNote.Id, creditNote.Number, creditNote.Date, creditNote.CustomerId, creditNote.Customer.Name,
               ResolvePaymentStatus(creditNote),
               creditNote.Rows.Select(MapResponseRow));

    private static GetCreditNotesResponseRow MapResponseRow(CreditNoteRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);

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
