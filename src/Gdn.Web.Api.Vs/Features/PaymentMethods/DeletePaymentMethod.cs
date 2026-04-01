using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.PaymentMethods;

public class DeletePaymentMethod
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/paymentmethods/{id:int}", Handler).WithTags(Tags.PaymentMethods);
        }
    }

    private static async Task<IResult> Handler(int id, IUnitOfWork unitOfWork)
    {
        var repository = unitOfWork.GetRepository<IPaymentMethodRepository>();
        await repository.RemoveAsync(id);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
