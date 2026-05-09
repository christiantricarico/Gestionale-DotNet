using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Products;

public class CreateProduct
{
    public record CreateProductRequest(
        string Code,
        string? Description,
        string Type,
        int? ProductCategoryId,
        int? MeasurementUnitId,
        int? TaxRateId);

    public record CreateProductResponse(
        int Id,
        string Code,
        string? Description,
        string Type);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/products", HandlerAsync).WithTags(Tags.Products);
        }
    }

    public sealed class Validator : AbstractValidator<CreateProductRequest>
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
        CreateProductRequest request,
        IValidator<CreateProductRequest> validator,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var existing = await productRepository.CountAsync(p => p.Code == request.Code);
        if (existing > 0)
            return ResultHelper.Conflict(ProductErrors.CodeAlreadyExists(request.Code));

        var product = MapProduct(request);
        unitOfWork.GetRepository<IProductRepository>().Add(product);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(new CreateProductResponse(product.Id, product.Code, product.Description, product.Type));
    }

    private static Product MapProduct(CreateProductRequest request) => new()
    {
        Code = request.Code,
        Description = request.Description,
        Type = request.Type,
        ProductCategoryId = request.ProductCategoryId,
        MeasurementUnitId = request.MeasurementUnitId,
        TaxRateId = request.TaxRateId
    };
}
