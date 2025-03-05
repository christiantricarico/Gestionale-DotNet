using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.MeasurementUnits;

public class GetMeasurementUnits
{
    public record Response(int Id, string Code, string? Name, string? Description);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/measurementunits", Handler).WithTags(Tags.MeasurementUnits);
        }
    }

    private static async Task<IResult> Handler(IMeasurementUnitRepository measurementUnitRepository)
    {
        var data = await measurementUnitRepository.GetAllAsync();
        var responseData = data.Select(e => MapResponse(e));

        return ResultHelper.Ok(responseData);
    }

    private static Response MapResponse(MeasurementUnit entity)
    {
        return new(entity.Id, entity.Code, entity.Name, entity.Description);
    }
}
