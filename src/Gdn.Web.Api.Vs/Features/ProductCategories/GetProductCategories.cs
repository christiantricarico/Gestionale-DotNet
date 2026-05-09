using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.ProductCategories;

public class GetProductCategories
{
    public record GetProductCategoriesResponse(
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
            app.MapGet("api/productcategories", HandlerAsync).WithTags(Tags.ProductCategories);
        }
    }

    private static async Task<IResult> HandlerAsync(IProductCategoryRepository productCategoryRepository)
    {
        var includes = new string[] { "ParentCategory" };
        var data = await productCategoryRepository.GetAllAsync(includes: includes);
        var responseData = data.Select(MapResponse);

        return ResultHelper.Ok(responseData);
    }

    private static GetProductCategoriesResponse MapResponse(ProductCategory entity) =>
        new(entity.Id, entity.Code, entity.Name, entity.Description, entity.Level,
            entity.ParentCategoryId, entity.ParentCategory?.Name);
}
