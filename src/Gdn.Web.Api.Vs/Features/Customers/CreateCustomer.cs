using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Customers;

public class CreateCustomer
{
    public record Request(string Code, string? Name, string? Description, string? FiscalCode, string? VatNumber);
    public record Response(int Id, string Code, string? Name, string? Description, string? FiscalCode, string? VatNumber);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/customers", Handler).WithTags(Tags.Customers);
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

        var customer = MapCustomer(request);

        var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();
        customerRepository.Add(customer);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(new Response(customer.Id, customer.Code, customer.Name, customer.Description,
            customer.FiscalCode, customer.VatNumber));
    }

    private static Customer MapCustomer(Request request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        Description = request.Description,
        FiscalCode = request.FiscalCode,
        VatNumber = request.VatNumber
    };
}
