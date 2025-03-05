using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.MeasurementUnits;

public class DeleteMeasurementUnit
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/measurementunits/{id:int}", Handler).WithTags(Tags.MeasurementUnits);
        }
    }

    private static async Task<IResult> Handler(int id, IUnitOfWork unitOfWork)
    {
        var measurementUnitRepository = unitOfWork.GetRepository<IMeasurementUnitRepository>();
        await measurementUnitRepository.RemoveAsync(id);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
