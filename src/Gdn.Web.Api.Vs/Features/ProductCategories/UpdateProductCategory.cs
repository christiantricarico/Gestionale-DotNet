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
        int Level,
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

        MapCategory(category, request);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(category));
    }

    private static void MapCategory(ProductCategory category, UpdateProductCategoryRequest request)
    {
        category.Code = request.Code;
        category.Name = request.Name;
        category.Description = request.Description;
        category.Level = request.Level;
        category.ParentCategoryId = request.ParentCategoryId;
    }

    private static UpdateProductCategoryResponse MapResponse(ProductCategory category) =>
        new(category.Id, category.Code, category.Name, category.Description, category.Level, category.ParentCategoryId);
}
