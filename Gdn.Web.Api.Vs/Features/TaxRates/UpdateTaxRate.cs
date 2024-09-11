using FluentValidation;
using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public static class UpdateTaxRate
{
    public record Request(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);
    public record Response(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("taxrates", Handler).WithTags(Tags.TaxRates);
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

        var taxRate = await context.TaxRates.FindAsync(request.Id);
        if (taxRate is null)
            return Results.NotFound(TaxRateErrors.NotFound(request.Id));

        taxRate.Code = request.Code;
        taxRate.Name = request.Name;
        taxRate.Description = request.Description;
        taxRate.Rate = request.Rate;
        taxRate.TaxRateNatureId = request.TaxRateNatureId;

        await context.SaveChangesAsync();

        return Results.Ok(new Response(taxRate.Id, taxRate.Code, taxRate.Name, taxRate.Description, taxRate.Rate, taxRate.TaxRateNatureId));
    }
}
