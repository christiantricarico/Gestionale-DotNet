using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Products;

public class GetProductById
{
    public record GetProductByIdResponse(
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
            app.MapGet("api/products/{id:int}", HandlerAsync).WithTags(Tags.Products);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IProductRepository productRepository)
    {
        var includes = new string[] { "ProductCategory", "MeasurementUnit", "TaxRate" };
        var data = await productRepository.GetAsync(id, includes);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound();
    }

    private static GetProductByIdResponse MapResponse(Product entity) =>
        new(entity.Id, entity.Code, entity.Description, entity.Type,
            entity.ProductCategoryId, entity.ProductCategory?.Name,
            entity.MeasurementUnitId, entity.MeasurementUnit?.Name,
            entity.TaxRateId, entity.TaxRate?.Code, entity.TaxRate is not null ? entity.TaxRate.Rate / 100 : null);
}
