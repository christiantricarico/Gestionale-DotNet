using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Customers;

public class UpdateCustomer
{
    public record Request(int Id, string Code, string? Name, string? Description);
    public record Response(int Id, string Code, string? Name, string? Description);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/customers", Handler).WithTags(Tags.Customers);
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

        var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();

        var customer = await customerRepository.GetAsync(request.Id);
        if (customer is null)
            return ResultHelper.NotFound(CustomerErrors.NotFound(request.Id));

        MapCustomer(customer, request);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(customer));
    }

    private static void MapCustomer(Customer customer, Request request)
    {
        customer.Code = request.Code;
        customer.Name = request.Name;
        customer.Description = request.Description;
    }

    private static Response MapResponse(Customer entity) =>
        new(entity.Id, entity.Code, entity.Name, entity.Description);
}
