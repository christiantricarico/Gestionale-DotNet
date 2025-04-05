using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Customers;

public class CreateCustomer
{
    public record Request(string Code, string? Name, string? Description, string? FiscalCode, string? VatNumber,
        string? Phone, string? Email, string? Website, string? Pec, string? Sdi, string? Notes,
        string? Street, string? PostalCode, string? City, string? Province, string? Country);
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

        var customer = MapCustomer(request);

        var address = MapAddress(request);
        customer.Addresses.Add(address);

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
        VatNumber = request.VatNumber,
        Phone = request.Phone,
        Email = request.Email,
        Website = request.Website,
        Pec = request.Pec,
        Sdi = request.Sdi,
        Notes = request.Notes
    };

    private static Address MapAddress(Request request) => new()
    {
        Street = request.Street,
        PostalCode = request.PostalCode,
        City = request.City,
        Province = request.Province,
        Country = request.Country
    };
}
