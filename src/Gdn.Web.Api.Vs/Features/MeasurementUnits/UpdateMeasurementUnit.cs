using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.MeasurementUnits;

public class UpdateMeasurementUnit
{
    public record Request(int Id, string Code, string? Name, string? Description);
    public record Response(int Id, string Code, string? Name, string? Description);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/measurementunits", Handler).WithTags(Tags.MeasurementUnits);
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

        var measurementUnitRepository = unitOfWork.GetRepository<IMeasurementUnitRepository>();

        var measurementUnit = await measurementUnitRepository.GetAsync(request.Id);
        if (measurementUnit is null)
            return ResultHelper.NotFound(MeasurementUnitErrors.NotFound(request.Id));

        MapMeasurementUnit(measurementUnit, request);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(measurementUnit));
    }

    private static void MapMeasurementUnit(MeasurementUnit measurementUnit, Request request)
    {
        measurementUnit.Code = request.Code;
        measurementUnit.Name = request.Name;
        measurementUnit.Description = request.Description;
    }

    private static Response MapResponse(MeasurementUnit entity) =>
        new(entity.Id, entity.Code, entity.Name, entity.Description);
}
