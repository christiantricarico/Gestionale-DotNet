using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.InterventionReports;

public class GetLastInterventionReportNumber
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/interventionreports/last-number", HandlerAsync).WithTags(Tags.InterventionReports);
        }
    }

    private static async Task<IResult> HandlerAsync(IInterventionReportRepository reportRepository)
    {
        var reports = await reportRepository.GetAllAsync();
        var lastNumber = reports
            .Select(i => int.TryParse(i.Number, out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        return ResultHelper.Ok(lastNumber);
    }
}
