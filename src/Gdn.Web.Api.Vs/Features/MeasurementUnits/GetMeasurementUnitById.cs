using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.MeasurementUnits;

public class GetMeasurementUnitById
{
    public record GetMeasurementUnitByIdResponse(int Id, string Code, string? Name, string? Description);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/measurementunits/{id:int}", HandlerAsync).WithTags(Tags.MeasurementUnits);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IMeasurementUnitRepository measurementUnitRepository)
    {
        var data = await measurementUnitRepository.GetAsync(id);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound();
    }

    private static GetMeasurementUnitByIdResponse MapResponse(MeasurementUnit entity)
    {
        return new(entity.Id, entity.Code, entity.Name, entity.Description);
    }
}
