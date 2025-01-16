using FluentValidation;
using Gdn.Domain.Models;
using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.ProductCategories;

public class CreateProductCategory
{
    public record Request(string Code, string? Name, string? Description, int? ParentCategoryId);
    public record Response(int Id, string Code, string? Name, string? Description, int Level, int? ParentCategoryId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/productcategories", Handler).WithTags(Tags.ProductCategories);
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

        var entity = new ProductCategory
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId
        };

        //set category level
        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await context.ProductCategories.FindAsync(request.ParentCategoryId.Value);
            entity.Level = parentCategory is null ? 0 : parentCategory.Level + 1;
        }

        context.ProductCategories.Add(entity);
        await context.SaveChangesAsync();

        return Results.Ok(new Response(entity.Id, entity.Code, entity.Name, entity.Description, entity.Level, entity.ParentCategoryId));
    }
}
