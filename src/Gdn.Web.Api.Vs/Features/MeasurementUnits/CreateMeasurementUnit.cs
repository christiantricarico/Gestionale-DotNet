using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.MeasurementUnits;

public class CreateMeasurementUnit
{
    public record Request(string Code, string? Name, string? Description);
    public record Response(int Id, string Code, string? Name, string? Description);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/measurementunits", Handler).WithTags(Tags.MeasurementUnits);
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

        var measurementUnit = MapMeasurementUnit(request);

        var measurementUnitRepository = unitOfWork.GetRepository<IMeasurementUnitRepository>();
        measurementUnitRepository.Add(measurementUnit);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(new Response(measurementUnit.Id, measurementUnit.Code, measurementUnit.Name, measurementUnit.Description));
    }

    private static MeasurementUnit MapMeasurementUnit(Request request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        Description = request.Description
    };
}
