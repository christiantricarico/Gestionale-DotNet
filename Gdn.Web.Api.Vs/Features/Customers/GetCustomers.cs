using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Customers;

public class GetCustomers
{
    public record Response(int Id, string Code, string? Name, string? Description);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/customers", Handler).WithTags(Tags.Customers);
        }
    }

    private static async Task<IResult> Handler(ICustomerRepository customerRepository)
    {
        var data = await customerRepository.GetAllAsync();
        var responseData = data.Select(e => MapResponse(e));

        return ResultHelper.Ok(responseData);
    }

    private static Response MapResponse(Customer entity)
    {
        return new(entity.Id, entity.Code, entity.Name, entity.Description);
    }
}
