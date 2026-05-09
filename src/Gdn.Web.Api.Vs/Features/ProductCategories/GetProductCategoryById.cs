using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.ProductCategories;

public class GetProductCategoryById
{
    public record GetProductCategoryByIdResponse(
        int Id,
        string Code,
        string? Name,
        string? Description,
        int Level,
        int? ParentCategoryId,
        string? ParentCategoryName);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/productcategories/{id:int}", HandlerAsync).WithTags(Tags.ProductCategories);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IProductCategoryRepository productCategoryRepository)
    {
        var includes = new string[] { "ParentCategory" };
        var data = await productCategoryRepository.GetAsync(id, includes);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound();
    }

    private static GetProductCategoryByIdResponse MapResponse(ProductCategory entity) =>
        new(entity.Id, entity.Code, entity.Name, entity.Description, entity.Level,
            entity.ParentCategoryId, entity.ParentCategory?.Name);
}
