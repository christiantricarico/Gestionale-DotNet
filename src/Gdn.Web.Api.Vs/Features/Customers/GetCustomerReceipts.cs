using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Customers;

/// <summary>
/// Returns the history of all receipts (incassi) registered for a given customer,
/// with details of the dues covered by each receipt.
/// </summary>
public class GetCustomerReceipts
{
    public record CoveredDueResponse(
        int DueId,
        DateOnly DueDate,
        string? DocumentNumber,
        string DocumentType,
        decimal AllocationAmount);

    public record Response(
        int PaymentId,
        DateOnly Date,
        decimal Amount,
        string? PaymentMethodName,
        IEnumerable<CoveredDueResponse> CoveredDues);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/customers/{customerId:int}/receipts", Handler).WithTags(Tags.Customers);
        }
    }

    private static async Task<IResult> Handler(int customerId, ICustomerRepository customerRepository, IPaymentRepository paymentRepository)
    {
        var customer = await customerRepository.GetAsync(customerId);
        if (customer is null)
            return ResultHelper.NotFound(CustomerErrors.NotFound(customerId));

        var payments = await paymentRepository.GetAllAsync(
            predicate: p => p.CustomerId == customerId && !p.IsDeleted,
            includes: ["PaymentMethod", "PaymentDues.Due.Invoice"]);

        var receipts = payments
            .OrderByDescending(p => p.Date)
            .Select(p => new Response(
                p.Id,
                p.Date,
                p.Amount,
                p.PaymentMethod?.Name,
                p.PaymentDues.Select(pd => new CoveredDueResponse(
                    pd.DueId,
                    pd.Due.Date,
                    pd.Due.Invoice?.Number,
                    pd.Due.Invoice is not null ? "Fattura" : "Scadenza",
                    pd.Amount))));

        return ResultHelper.Ok(receipts);
    }
}
