using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.InterventionReports;

public class GetAvailableInterventionReportsByCustomer
{
    public record Response(int Id, string Number, DateOnly Date, decimal TotalAmount);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/interventionreports/customer/{customerId:int}/available", HandlerAsync).WithTags(Tags.InterventionReports);
        }
    }

    private static async Task<IResult> HandlerAsync(int customerId, int? invoiceId, IInterventionReportRepository reportRepository)
    {
        var reports = await reportRepository.GetAllAsync(
            includes: ["Rows.TaxRate"],
            predicate: report => report.CustomerId == customerId
                && (!report.IsInvoiced || report.InvoiceId == invoiceId));

        return ResultHelper.Ok(reports.Select(r => new Response(r.Id, r.Number, r.Date, InterventionReportAmountCalculator.CalculateTotal(r))));
    }
}
