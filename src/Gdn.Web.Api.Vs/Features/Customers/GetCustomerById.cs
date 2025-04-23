using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Customers;

public class GetCustomerById
{
    public record Response(int Id, string Code, string? Name, string? Description, string? FiscalCode, string? VatNumber,
        string? Phone, string? Email, string? Website, string? Pec, string? Sdi, string? Notes,
        string? Street, string? PostalCode, string? City, string? Province, string? Country);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/customers/{id}", Handler).WithTags(Tags.Customers);
        }
    }

    private static async Task<IResult> Handler(ICustomerRepository customerRepository, int id)
    {
        var data = await customerRepository.GetAsync(id, ["Addresses"]);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound();
    }

    private static Response MapResponse(Customer entity)
    {
        var address = entity.Addresses.FirstOrDefault();

        return new(entity.Id, entity.Code, entity.Name, entity.Description, entity.FiscalCode, entity.VatNumber,
            entity.Phone, entity.Email, entity.Website, entity.Pec, entity.Sdi, entity.Notes,
            address?.Street, address?.PostalCode, address?.City, address?.Province, address?.Country);
    }
}
