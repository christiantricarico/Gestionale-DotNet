using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Interventions;

public class GetAvailableInterventionsByCustomer
{
    public record Response(int Id, string Number, DateOnly Date, decimal TotalAmount, decimal ExemptVatTotal);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/interventions/customer/{customerId:int}/available", HandlerAsync).WithTags(Tags.Interventions);
        }
    }

    private static async Task<IResult> HandlerAsync(int customerId, int? invoiceId, IInterventionRepository reportRepository)
    {
        var reports = await reportRepository.GetAllAsync(
            includes: ["Rows.TaxRate"],
            predicate: report => report.CustomerId == customerId
                && (!report.IsInvoiced || report.InvoiceId == invoiceId));

        return ResultHelper.Ok(reports.Select(r => new Response(
            r.Id, r.Number, r.Date,
            InterventionAmountCalculator.CalculateTotal(r),
            InterventionAmountCalculator.CalculateExemptVatTotal(r))));
    }
}
