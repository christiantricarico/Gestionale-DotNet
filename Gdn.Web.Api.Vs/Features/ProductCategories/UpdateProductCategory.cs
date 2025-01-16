using FluentValidation;
using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.ProductCategories;

public class UpdateProductCategory
{
    public record Request(int Id, string Code, string? Name, string? Description, int? ParentCategoryId);
    public record Response(int Id, string Code, string? Name, string? Description, int Level, int? ParentCategoryId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/productcategories", Handler).WithTags(Tags.ProductCategories);
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(e => e.Code).NotEmpty().MaximumLength(10);
            RuleFor(e => e.Name).MaximumLength(255);
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);

        var productCategory = await context.ProductCategories.FindAsync(request.Id);
        if (productCategory is null)
            return Results.NotFound(ProductCategoryErrors.NotFound(request.Id));

        productCategory.Code = request.Code;
        productCategory.Name = request.Name;
        productCategory.Description = request.Description;
        productCategory.ParentCategoryId = request.ParentCategoryId;

        await context.SaveChangesAsync();

        return Results.Ok(new Response(productCategory.Id, productCategory.Code, productCategory.Name, productCategory.Description, productCategory.Level, productCategory.ParentCategoryId));
    }
}
