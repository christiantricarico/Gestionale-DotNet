using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Payments;

public class DeletePayment
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/payments/{id:int}", HandlerAsync).WithTags(Tags.Payments);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IUnitOfWork unitOfWork)
    {
        var paymentRepository = unitOfWork.GetRepository<IPaymentRepository>();

        // Load with PaymentDues so EF Core cascades the delete in-memory,
        // causing the TR_PaymentDues_AfterDelete trigger to fire and update Due.PaidAmount.
        var payment = await paymentRepository.GetAsync(id, ["PaymentDues"]);
        if (payment is null)
            return ResultHelper.NotFound(PaymentErrors.NotFound(id));

        await paymentRepository.RemoveAsync(id);
        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
