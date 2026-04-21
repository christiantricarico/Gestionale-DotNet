using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.InterventionReports;

public class GetInterventionReports
{
    public record Response(int Id, string Number, DateOnly Date, int CustomerId, string? CustomerName, bool IsInvoiced, int? InvoiceId, string? InvoiceNumber, decimal TotalAmount);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/interventionreports", HandlerAsync).WithTags(Tags.InterventionReports);
        }
    }

    private static async Task<IResult> HandlerAsync(int? customerId, bool? isInvoiced, IInterventionReportRepository reportRepository)
    {
        var data = await reportRepository.GetAllAsync(
            includes: ["Customer", "Rows.TaxRate", "Invoice"],
            predicate: report => (!customerId.HasValue || report.CustomerId == customerId.Value)
                && (!isInvoiced.HasValue || report.IsInvoiced == isInvoiced.Value));

        return ResultHelper.Ok(data.Select(MapResponse));
    }

    private static Response MapResponse(InterventionReport report)
        => new(
            report.Id,
            report.Number,
            report.Date,
            report.CustomerId,
            report.Customer.Name,
            report.IsInvoiced,
            report.InvoiceId,
            report.Invoice?.Number,
            InterventionReportAmountCalculator.CalculateTotal(report));
}
