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
        int Level,
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

        var category = MapCategory(request);
        unitOfWork.GetRepository<IProductCategoryRepository>().Add(category);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(new CreateProductCategoryResponse(
            category.Id, category.Code, category.Name, category.Description, category.Level));
    }

    private static ProductCategory MapCategory(CreateProductCategoryRequest request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        Description = request.Description,
        Level = request.Level,
        ParentCategoryId = request.ParentCategoryId
    };
}
