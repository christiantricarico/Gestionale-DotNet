using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Interventions;

public class GetInterventions
{
    public record Response(int Id, string Number, DateOnly Date, int CustomerId, string? CustomerName, bool IsInvoiced, int? InvoiceId, string? InvoiceNumber, decimal TotalAmount);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/interventions", HandlerAsync).WithTags(Tags.Interventions);
        }
    }

    private static async Task<IResult> HandlerAsync(int? customerId, bool? isInvoiced, IInterventionRepository reportRepository)
    {
        var data = await reportRepository.GetAllAsync(
            includes: ["Customer", "Rows.TaxRate", "Invoice"],
            predicate: report => (!customerId.HasValue || report.CustomerId == customerId.Value)
                && (!isInvoiced.HasValue || report.IsInvoiced == isInvoiced.Value));

        return ResultHelper.Ok(data.Select(MapResponse));
    }

    private static Response MapResponse(Intervention report)
        => new(
            report.Id,
            report.Number,
            report.Date,
            report.CustomerId,
            report.Customer.Name,
            report.IsInvoiced,
            report.InvoiceId,
            report.Invoice?.Number,
            InterventionAmountCalculator.CalculateTotal(report));
}
