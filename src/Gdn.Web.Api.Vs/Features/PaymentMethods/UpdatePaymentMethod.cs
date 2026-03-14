using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.PaymentMethods;

public class UpdatePaymentMethod
{
    public record Request(int Id, string Code, string? Name, string? Description, string? DigitalInvoiceCode);
    public record Response(int Id, string Code, string? Name, string? Description, string? DigitalInvoiceCode);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/paymentmethods", Handler).WithTags(Tags.PaymentMethods);
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(e => e.Code).NotEmpty().MaximumLength(10);
            RuleFor(e => e.Name).MaximumLength(255);
            RuleFor(e => e.Description).MaximumLength(500);
            RuleFor(e => e.DigitalInvoiceCode).MaximumLength(4);
        }
    }

    private static async Task<IResult> Handler(Request request, IValidator<Request> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var repository = unitOfWork.GetRepository<IPaymentMethodRepository>();

        var entity = await repository.GetAsync(request.Id);
        if (entity is null)
            return ResultHelper.NotFound(PaymentMethodErrors.NotFound(request.Id));

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.DigitalInvoiceCode = request.DigitalInvoiceCode;

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(entity));
    }

    private static Response MapResponse(PaymentMethod e) =>
        new(e.Id, e.Code, e.Name, e.Description, e.DigitalInvoiceCode);
}
