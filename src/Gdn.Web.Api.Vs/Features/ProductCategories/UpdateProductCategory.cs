using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.ProductCategories;

public class UpdateProductCategory
{
    public record UpdateProductCategoryRequest(
        int Id,
        string Code,
        string? Name,
        string? Description,
        int? ParentCategoryId);

    public record UpdateProductCategoryResponse(
        int Id,
        string Code,
        string? Name,
        string? Description,
        int Level,
        int? ParentCategoryId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/productcategories", HandlerAsync).WithTags(Tags.ProductCategories);
        }
    }

    public sealed class Validator : AbstractValidator<UpdateProductCategoryRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Code).NotEmpty().MaximumLength(10);
            RuleFor(e => e.Name).MaximumLength(255);
            RuleFor(e => e.Description).MaximumLength(1000);
        }
    }

    private static async Task<IResult> HandlerAsync(
        UpdateProductCategoryRequest request,
        IValidator<UpdateProductCategoryRequest> validator,
        IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var categoryRepository = unitOfWork.GetRepository<IProductCategoryRepository>();

        var category = await categoryRepository.GetAsync(request.Id);
        if (category is null)
            return ResultHelper.NotFound(ProductCategoryErrors.NotFound(request.Id));

        var categoriesById = (await categoryRepository.GetAllAsync()).ToDictionary(category => category.Id);
        var levelResult = ResolveLevel(request, categoriesById);
        if (levelResult.IsFailure)
        {
            return ResultHelper.BadRequest(levelResult.Error);
        }

        MapCategory(category, request, levelResult.Value);
        categoriesById[category.Id] = category;

        UpdateDescendantLevels(category, categoriesById);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(category));
    }

    private static void MapCategory(ProductCategory category, UpdateProductCategoryRequest request, int level)
    {
        category.Code = request.Code;
        category.Name = request.Name;
        category.Description = request.Description;
        category.Level = level;
        category.ParentCategoryId = request.ParentCategoryId;
    }

    private static LevelResult ResolveLevel(
        UpdateProductCategoryRequest request,
        Dictionary<int, ProductCategory> categoriesById)
    {
        if (request.ParentCategoryId is null)
        {
            return LevelResult.Success(0);
        }

        if (request.ParentCategoryId.Value == request.Id)
        {
            return LevelResult.Failure(ProductCategoryErrors.CircularReference(request.ParentCategoryId.Value));
        }

        if (!categoriesById.TryGetValue(request.ParentCategoryId.Value, out var parentCategory))
        {
            return LevelResult.Failure(ProductCategoryErrors.ParentNotFound(request.ParentCategoryId.Value));
        }

        if (CreatesCircularReference(request.Id, parentCategory.Id, categoriesById))
        {
            return LevelResult.Failure(ProductCategoryErrors.CircularReference(parentCategory.Id));
        }

        return LevelResult.Success(parentCategory.Level + 1);
    }

    private static bool CreatesCircularReference(
        int categoryId,
        int selectedParentId,
        Dictionary<int, ProductCategory> categoriesById)
    {
        var currentParentId = selectedParentId;
        HashSet<int> visitedCategoryIds = [];

        while (true)
        {
            if (currentParentId == categoryId)
            {
                return true;
            }

            if (!visitedCategoryIds.Add(currentParentId))
            {
                return true;
            }

            if (!categoriesById.TryGetValue(currentParentId, out var currentCategory) ||
                currentCategory.ParentCategoryId is null)
            {
                return false;
            }

            currentParentId = currentCategory.ParentCategoryId.Value;
        }
    }

    private static void UpdateDescendantLevels(
        ProductCategory category,
        Dictionary<int, ProductCategory> categoriesById)
    {
        var childCategoriesByParentId = categoriesById.Values
            .Where(currentCategory => currentCategory.ParentCategoryId is not null)
            .GroupBy(currentCategory => currentCategory.ParentCategoryId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        Stack<ProductCategory> categoriesToVisit = [];
        categoriesToVisit.Push(category);

        while (categoriesToVisit.Count > 0)
        {
            var currentCategory = categoriesToVisit.Pop();
            if (!childCategoriesByParentId.TryGetValue(currentCategory.Id, out var childCategories))
            {
                continue;
            }

            foreach (var childCategory in childCategories)
            {
                childCategory.Level = currentCategory.Level + 1;
                categoriesToVisit.Push(childCategory);
            }
        }
    }

    private static UpdateProductCategoryResponse MapResponse(ProductCategory category) =>
        new(category.Id, category.Code, category.Name, category.Description, category.Level, category.ParentCategoryId);

    private readonly record struct LevelResult(int Value, Error? Error)
    {
        public bool IsFailure => Error is not null;

        public static LevelResult Success(int value) => new(value, null);
        public static LevelResult Failure(Error error) => new(default, error);
    }
}
