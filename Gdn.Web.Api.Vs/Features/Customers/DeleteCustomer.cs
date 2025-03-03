using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Customers;

public class DeleteCustomer
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/customers/{id:int}", Handler).WithTags(Tags.Customers);
        }
    }

    private static async Task<IResult> Handler(int id, IUnitOfWork unitOfWork)
    {
        var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();
        await customerRepository.RemoveAsync(id);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
