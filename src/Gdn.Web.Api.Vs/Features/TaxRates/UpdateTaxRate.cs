using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public class UpdateTaxRate
{
    public record UpdateTaxRateRequest(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);
    public record UpdateTaxRateResponse(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/taxrates", HandlerAsync).WithTags(Tags.TaxRates);
        }
    }

    public sealed class Validator : AbstractValidator<UpdateTaxRateRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Code).NotEmpty().MaximumLength(10);
            RuleFor(e => e.Name).MaximumLength(255);
        }
    }

    private static async Task<IResult> HandlerAsync(UpdateTaxRateRequest request, IValidator<UpdateTaxRateRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();

        var taxRate = await taxRateRepository.GetAsync(request.Id);
        if (taxRate is null)
            return ResultHelper.NotFound(TaxRateErrors.NotFound(request.Id));

        MapTaxRate(taxRate, request);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(taxRate));
    }

    private static void MapTaxRate(TaxRate taxRate, UpdateTaxRateRequest request)
    {
        taxRate.Code = request.Code;
        taxRate.Name = request.Name;
        taxRate.Description = request.Description;
        taxRate.Rate = request.Rate;
        taxRate.TaxRateNatureId = request.TaxRateNatureId;
    }

    private static UpdateTaxRateResponse MapResponse(TaxRate taxRate) =>
        new(taxRate.Id, taxRate.Code, taxRate.Name, taxRate.Description, taxRate.Rate, taxRate.TaxRateNatureId);
}
