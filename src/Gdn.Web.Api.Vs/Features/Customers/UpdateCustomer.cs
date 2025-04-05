using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Customers;

public class UpdateCustomer
{
    public record Request(int Id, string Code, string? Name, string? Description, string? FiscalCode, string? VatNumber,
        string? Phone, string? Email, string? Website, string? Pec, string? Sdi, string? Notes,
        string? Street, string? PostalCode, string? City, string? Province, string? Country);
    public record Response(int Id, string Code, string? Name, string? Description, string? FiscalCode, string? VatNumber);

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
            RuleFor(e => e.FiscalCode).MaximumLength(20);
            RuleFor(e => e.VatNumber).MaximumLength(20);
            RuleFor(e => e.Phone).MaximumLength(50);
            RuleFor(e => e.Email).MaximumLength(255);
            RuleFor(e => e.Website).MaximumLength(255);
            RuleFor(e => e.Pec).MaximumLength(255);
            RuleFor(e => e.Sdi).MaximumLength(10);
            RuleFor(e => e.Street).MaximumLength(255);
            RuleFor(e => e.PostalCode).MaximumLength(10);
            RuleFor(e => e.City).MaximumLength(50);
            RuleFor(e => e.Province).MaximumLength(50);
            RuleFor(e => e.Country).MaximumLength(50);
        }
    }

    private static async Task<IResult> Handler(Request request, IValidator<Request> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();

        IEnumerable<string> includes = ["Addresses"];
        var customer = await customerRepository.GetAsync(request.Id, includes);
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
        customer.FiscalCode = request.FiscalCode;
        customer.VatNumber = request.VatNumber;
        customer.Phone = request.Phone;
        customer.Email = request.Email;
        customer.Website = request.Website;
        customer.Pec = request.Pec;
        customer.Sdi = request.Sdi;
        customer.Notes = request.Notes;

        MapAddress(customer, request);
    }

    private static void MapAddress(Customer customer, Request request)
    {
        var address = customer.Addresses.FirstOrDefault();
        if (address is null)
        {
            address = new Address();
            customer.Addresses.Add(address);
        }

        address.Street = request.Street;
        address.PostalCode = request.PostalCode;
        address.City = request.City;
        address.Province = request.Province;
        address.Country = request.Country;
    }

    private static Response MapResponse(Customer entity) =>
        new(entity.Id, entity.Code, entity.Name, entity.Description, entity.FiscalCode, entity.VatNumber);
}
