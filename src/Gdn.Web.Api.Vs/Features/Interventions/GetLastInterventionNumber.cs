using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Interventions;

public class GetLastInterventionNumber
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/interventions/last-number", HandlerAsync).WithTags(Tags.Interventions);
        }
    }

    private static async Task<IResult> HandlerAsync(IInterventionRepository reportRepository)
    {
        var reports = await reportRepository.GetAllAsync();
        var lastNumber = reports
            .Select(i => int.TryParse(i.Number, out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        return ResultHelper.Ok(lastNumber);
    }
}
