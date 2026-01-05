using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public class CreateTaxRate
{
    public record CreateTaxRateRequest(string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);
    public record CreateTaxRateResponse(int Id, string Code, string? Name, string? Description, decimal Rate);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/taxrates", Handler).WithTags(Tags.TaxRates);
        }
    }

    public sealed class Validator : AbstractValidator<CreateTaxRateRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Code).NotEmpty().MaximumLength(10);
            RuleFor(e => e.Name).MaximumLength(255);
        }
    }

    private static async Task<IResult> Handler(CreateTaxRateRequest request, IValidator<CreateTaxRateRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var taxRate = MapTaxRate(request);

        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();
        taxRateRepository.Add(taxRate);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(new CreateTaxRateResponse(taxRate.Id, taxRate.Code, taxRate.Name, taxRate.Description, taxRate.Rate));
    }

    private static TaxRate MapTaxRate(CreateTaxRateRequest request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        Description = request.Description,
        Rate = request.Rate,
        TaxRateNatureId = request.TaxRateNatureId
    };
}