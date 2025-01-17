using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public class UpdateTaxRate
{
    public record Request(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);
    public record Response(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/taxrates", Handler).WithTags(Tags.TaxRates);
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

    private static async Task<IResult> Handler(Request request, IValidator<Request> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);

        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();

        var taxRate = await taxRateRepository.GetAsync(request.Id);
        if (taxRate is null)
            return Results.NotFound(TaxRateErrors.NotFound(request.Id));

        taxRate.Code = request.Code;
        taxRate.Name = request.Name;
        taxRate.Description = request.Description;
        taxRate.Rate = request.Rate;
        taxRate.TaxRateNatureId = request.TaxRateNatureId;

        await unitOfWork.SaveChangesAsync();

        return Results.Ok(new Response(taxRate.Id, taxRate.Code, taxRate.Name, taxRate.Description, taxRate.Rate, taxRate.TaxRateNatureId));
    }
}
