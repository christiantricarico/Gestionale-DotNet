using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public static class GetTaxRates
{
    public record Response(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/taxrates", Handler).WithTags(Tags.TaxRates);
        }
    }

    public static async Task<IResult> Handler(ITaxRateRepository taxRateRepository)
    {
        var data = await taxRateRepository.GetAllAsync();
        var responseData = data.Select(e => new Response(e.Id, e.Code, e.Name, e.Description, e.Rate, e.TaxRateNatureId));

        return TypedResults.Ok(responseData);
    }
}
