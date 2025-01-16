using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Gdn.Web.Api.Vs.Features.ProductCategories;

public class GetProductCategoryById
{
    public record Response(int Id, string Code, string? Name, string? Description, int Level, int? ParentCategoryId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/productcategories/{id}", Handler).WithTags(Tags.ProductCategories);
        }
    }

    public static async Task<IResult> Handler(AppDbContext context, int id)
    {
        var productCategory = await context.ProductCategories
            .Where(e => e.Id == id)
            .Select(e => new Response(e.Id, e.Code, e.Name, e.Description, e.Level, e.ParentCategoryId))
            .FirstOrDefaultAsync();

        return productCategory is not null
            ? TypedResults.Ok(productCategory)
            : TypedResults.NotFound();
    }
}
