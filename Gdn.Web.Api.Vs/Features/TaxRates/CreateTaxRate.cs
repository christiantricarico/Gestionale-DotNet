using FluentValidation;
using Gdn.Domain.Models;
using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public static class CreateTaxRate
{
    public record Request(string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);
    public record Response(int Id, string Code, string? Name, string? Description, decimal Rate);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("taxrates", Handler).WithTags(Tags.TaxRates);
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(e => e.Code).NotEmpty().MaximumLength(10);
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);

        var taxRate = new TaxRate
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Rate = request.Rate,
            TaxRateNatureId = request.TaxRateNatureId
        };

        await context.TaxRates.AddAsync(taxRate);
        await context.SaveChangesAsync();

        return Results.Ok(new Response(taxRate.Id, taxRate.Code, taxRate.Name, taxRate.Description, taxRate.Rate));
    }
}
