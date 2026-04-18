using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class GetPaymentsByCreditNote
{
    public record AllocationResponse(int DueId, DateOnly DueDate, decimal AllocationAmount);
    public record Response(int Id, DateOnly Date, decimal Amount, int? PaymentMethodId, string? PaymentMethodName, IEnumerable<AllocationResponse> Allocations);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/creditnotes/{creditNoteId:int}/payments", HandlerAsync).WithTags(Tags.CreditNotes);
        }
    }

    private static async Task<IResult> HandlerAsync(int creditNoteId, IDueRepository dueRepository)
    {
        var dues = await dueRepository.GetAllAsync(
            predicate: d => d.CreditNoteId == creditNoteId,
            includes: ["PaymentDues.Payment.PaymentMethod"]);

        var payments = dues
            .SelectMany(d => d.PaymentDues)
            .GroupBy(pd => pd.PaymentId)
            .Select(g =>
            {
                var firstPd = g.First();
                var payment = firstPd.Payment;
                return new Response(
                    payment.Id,
                    payment.Date,
                    payment.Amount,
                    payment.PaymentMethodId,
                    payment.PaymentMethod?.Name,
                    g.Select(pd => new AllocationResponse(pd.DueId, pd.Due.Date, pd.Amount)));
            });

        return ResultHelper.Ok(payments);
    }
}
