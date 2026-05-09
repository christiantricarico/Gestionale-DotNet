using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.ProductCategories;

public class CreateProductCategory
{
    public record CreateProductCategoryRequest(
        string Code,
        string? Name,
        string? Description,
        int? ParentCategoryId);

    public record CreateProductCategoryResponse(int Id, string Code, string? Name, string? Description, int Level);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/productcategories", HandlerAsync).WithTags(Tags.ProductCategories);
        }
    }

    public sealed class Validator : AbstractValidator<CreateProductCategoryRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Code).NotEmpty().MaximumLength(10);
            RuleFor(e => e.Name).MaximumLength(255);
            RuleFor(e => e.Description).MaximumLength(1000);
        }
    }

    private static async Task<IResult> HandlerAsync(
        CreateProductCategoryRequest request,
        IValidator<CreateProductCategoryRequest> validator,
        IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var categoryRepository = unitOfWork.GetRepository<IProductCategoryRepository>();
        var levelResult = await ResolveLevelAsync(request.ParentCategoryId, categoryRepository);
        if (levelResult.IsFailure)
        {
            return ResultHelper.BadRequest(levelResult.Error);
        }

        var category = MapCategory(request, levelResult.Value);
        unitOfWork.GetRepository<IProductCategoryRepository>().Add(category);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(new CreateProductCategoryResponse(
            category.Id, category.Code, category.Name, category.Description, category.Level));
    }

    private static ProductCategory MapCategory(CreateProductCategoryRequest request, int level) => new()
    {
        Code = request.Code,
        Name = request.Name,
        Description = request.Description,
        Level = level,
        ParentCategoryId = request.ParentCategoryId
    };

    private static async Task<LevelResult> ResolveLevelAsync(
        int? parentCategoryId,
        IProductCategoryRepository categoryRepository)
    {
        if (parentCategoryId is null)
        {
            return LevelResult.Success(0);
        }

        var parentCategory = await categoryRepository.GetAsync(parentCategoryId.Value);
        if (parentCategory is null)
        {
            return LevelResult.Failure(ProductCategoryErrors.ParentNotFound(parentCategoryId.Value));
        }

        return LevelResult.Success(parentCategory.Level + 1);
    }

    private readonly record struct LevelResult(int Value, Error? Error)
    {
        public bool IsFailure => Error is not null;

        public static LevelResult Success(int value) => new(value, null);
        public static LevelResult Failure(Error error) => new(default, error);
    }
}
