using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.InterventionReports;

public class DeleteInterventionReport
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/interventionreports/{id:int}", HandlerAsync).WithTags(Tags.InterventionReports);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IUnitOfWork unitOfWork)
    {
        var reportRepository = unitOfWork.GetRepository<IInterventionReportRepository>();
        var report = await reportRepository.GetAsync(id);
        if (report is null)
            return ResultHelper.NotFound(InterventionReportErrors.NotFound(id));

        if (report.IsInvoiced)
            return ResultHelper.Conflict(InterventionReportErrors.AlreadyInvoiced(id));

        await reportRepository.RemoveAsync(id);
        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
