using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Products;

public class UpdateProduct
{
    public record UpdateProductRequest(
        int Id,
        string Code,
        string? Description,
        string Type,
        int? ProductCategoryId,
        int? MeasurementUnitId,
        int? TaxRateId);

    public record UpdateProductResponse(
        int Id,
        string Code,
        string? Description,
        string Type,
        int? ProductCategoryId,
        int? MeasurementUnitId,
        int? TaxRateId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/products", HandlerAsync).WithTags(Tags.Products);
        }
    }

    public sealed class Validator : AbstractValidator<UpdateProductRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Code).NotEmpty().MaximumLength(50);
            RuleFor(e => e.Description).MaximumLength(1000);
            RuleFor(e => e.Type).NotEmpty().MaximumLength(3)
                .Must(t => t == ProductType.Product || t == ProductType.Service)
                .WithMessage($"Type must be '{ProductType.Product}' or '{ProductType.Service}'");
        }
    }

    private static async Task<IResult> HandlerAsync(
        UpdateProductRequest request,
        IValidator<UpdateProductRequest> validator,
        IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var productRepository = unitOfWork.GetRepository<IProductRepository>();

        var product = await productRepository.GetAsync(request.Id);
        if (product is null)
            return ResultHelper.NotFound(ProductErrors.NotFound(request.Id));

        var codeConflict = await productRepository.CountAsync(p => p.Code == request.Code && p.Id != request.Id);
        if (codeConflict > 0)
            return ResultHelper.Conflict(ProductErrors.CodeAlreadyExists(request.Code));

        MapProduct(product, request);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(product));
    }

    private static void MapProduct(Product product, UpdateProductRequest request)
    {
        product.Code = request.Code;
        product.Description = request.Description;
        product.Type = request.Type;
        product.ProductCategoryId = request.ProductCategoryId;
        product.MeasurementUnitId = request.MeasurementUnitId;
        product.TaxRateId = request.TaxRateId;
    }

    private static UpdateProductResponse MapResponse(Product product) =>
        new(product.Id, product.Code, product.Description, product.Type,
            product.ProductCategoryId, product.MeasurementUnitId, product.TaxRateId);
}
