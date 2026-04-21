using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Interventions;

public class DeleteIntervention
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/interventions/{id:int}", HandlerAsync).WithTags(Tags.Interventions);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IUnitOfWork unitOfWork)
    {
        var reportRepository = unitOfWork.GetRepository<IInterventionRepository>();
        var report = await reportRepository.GetAsync(id);
        if (report is null)
            return ResultHelper.NotFound(InterventionErrors.NotFound(id));

        if (report.IsInvoiced)
            return ResultHelper.Conflict(InterventionErrors.AlreadyInvoiced(id));

        await reportRepository.RemoveAsync(id);
        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
