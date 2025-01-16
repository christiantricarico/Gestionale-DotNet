using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Gdn.Web.Api.Vs.Features.ProductCategories;

public class GetProductCategories
{
    public record Response(int Id, string Code, string? Name, string? Description, int Level, int? ParentCategoryId, string? ParentCategoryName);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/productcategories", Handler).WithTags(Tags.ProductCategories);
        }
    }

    public static async Task<IResult> Handler(AppDbContext context)
    {
        var data = await context.ProductCategories
            .Include(e => e.ParentCategory)
            .Select(e => new Response(e.Id, e.Code, e.Name, e.Description, e.Level, e.ParentCategoryId, e.ParentCategory != null ? e.ParentCategory.Name : default))
            .ToListAsync();

        return TypedResults.Ok(data);
    }
}
