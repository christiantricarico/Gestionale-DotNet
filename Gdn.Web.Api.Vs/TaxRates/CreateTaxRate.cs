using FluentValidation;
using Gdn.Domain.Models;
using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.TaxRates;

public static class CreateTaxRate
{
    public record Request(string Code, string? Name, string? Description, decimal Rate);
    public record Response(TaxRate taxRate);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(e => e.Code).NotEmpty().MaximumLength(10);
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("taxrates", Handler).WithTags("TaxRates");
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var taxRate = new TaxRate
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Rate = request.Rate
        };

        await context.TaxRates.AddAsync(taxRate);
        await context.SaveChangesAsync();

        return Results.Ok(new Response(taxRate));
    }
}
