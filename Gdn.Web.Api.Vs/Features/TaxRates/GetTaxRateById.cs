using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public static class GetTaxRateById
{
    public record Response(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/taxrates/{id}", Handler).WithTags(Tags.TaxRates);
        }
    }

    public static async Task<IResult> Handler(AppDbContext context, int id)
    {
        var taxRate = await context.TaxRates
            .Where(e => e.Id == id)
            .Select(e => new Response(e.Id, e.Code, e.Name, e.Description, e.Rate, e.TaxRateNatureId))
            .FirstOrDefaultAsync();

        return taxRate is not null
            ? TypedResults.Ok(taxRate)
            : TypedResults.NotFound();
    }
}
