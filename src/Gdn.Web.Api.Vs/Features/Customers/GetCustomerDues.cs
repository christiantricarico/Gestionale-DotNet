using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Customers;

/// <summary>
/// Returns all open (not fully paid) dues for a given customer, including dues from
/// invoices (positive amounts) and any standalone customer dues.
/// Ordered by due date ascending (oldest first) to support FIFO allocation in the receipt dialog.
/// </summary>
public class GetCustomerDues
{
    public record Response(
        int DueId,
        DateOnly DueDate,
        string? DocumentNumber,
        string DocumentType,
        decimal Amount,
        decimal PaidAmount,
        decimal RemainingAmount);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/customers/{customerId:int}/dues", Handler).WithTags(Tags.Customers);
        }
    }

    private static async Task<IResult> Handler(int customerId, ICustomerRepository customerRepository, IDueRepository dueRepository)
    {
        var customer = await customerRepository.GetAsync(customerId);
        if (customer is null)
            return ResultHelper.NotFound(CustomerErrors.NotFound(customerId));

        var allDues = await dueRepository.GetAllAsync(
            predicate: d => d.CustomerId == customerId && !d.IsDeleted,
            includes: ["Invoice", "CreditNote"]);

        var openDues = allDues
            .Where(d => !d.IsPaid)
            .OrderBy(d => d.Date)
            .Select(MapResponse);

        return ResultHelper.Ok(openDues);
    }

    private static Response MapResponse(Due d)
    {
        string documentType;
        string? documentNumber;

        if (d.Invoice is not null)
        {
            documentType = "Fattura";
            documentNumber = d.Invoice.Number;
        }
        else if (d.CreditNote is not null)
        {
            documentType = "Nota di credito";
            documentNumber = d.CreditNote.Number;
        }
        else
        {
            documentType = "Scadenza";
            documentNumber = null;
        }

        return new Response(d.Id, d.Date, documentNumber, documentType, d.Amount, d.PaidAmount, d.RemainingAmount);
    }
}
