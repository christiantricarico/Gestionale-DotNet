using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public class CreateTaxRate
{
    public record Request(string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);
    public record Response(int Id, string Code, string? Name, string? Description, decimal Rate);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/taxrates", Handler).WithTags(Tags.TaxRates);
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
            return ResultHelper.BadRequest(validationResult.Errors);

        var taxRate = new TaxRate
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Rate = request.Rate,
            TaxRateNatureId = request.TaxRateNatureId
        };

        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();
        taxRateRepository.Add(taxRate);
        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(new Response(taxRate.Id, taxRate.Code, taxRate.Name, taxRate.Description, taxRate.Rate));
    }
}
