using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public class GetTaxRateById
{
    public record GetTaxRateByIdResponse(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/taxrates/{id:int}", Handler).WithTags(Tags.TaxRates);
        }
    }

    private static async Task<IResult> Handler(int id, ITaxRateRepository taxRateRepository)
    {
        var data = await taxRateRepository.GetAsync(id);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound();
    }

    private static GetTaxRateByIdResponse MapResponse(TaxRate entity)
    {
        return new(entity.Id, entity.Code, entity.Name, entity.Description, entity.Rate, entity.TaxRateNatureId);
    }
}