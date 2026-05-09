using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Products;

public class GetProducts
{
    public record GetProductsResponse(
        int Id,
        string Code,
        string? Description,
        string Type,
        int? ProductCategoryId,
        string? ProductCategoryName,
        int? MeasurementUnitId,
        string? MeasurementUnitName,
        int? TaxRateId,
        string? TaxRateCode,
        decimal? TaxRateValue);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/products", HandlerAsync).WithTags(Tags.Products);
        }
    }

    private static async Task<IResult> HandlerAsync(IProductRepository productRepository)
    {
        var includes = new string[] { "ProductCategory", "MeasurementUnit", "TaxRate" };
        var data = await productRepository.GetAllAsync(includes: includes);
        var responseData = data.Select(MapResponse);

        return ResultHelper.Ok(responseData);
    }

    private static GetProductsResponse MapResponse(Product entity) =>
        new(entity.Id, entity.Code, entity.Description, entity.Type,
            entity.ProductCategoryId, entity.ProductCategory?.Name,
            entity.MeasurementUnitId, entity.MeasurementUnit?.Name,
            entity.TaxRateId, entity.TaxRate?.Code, entity.TaxRate is not null ? entity.TaxRate.Rate / 100 : null);
}
