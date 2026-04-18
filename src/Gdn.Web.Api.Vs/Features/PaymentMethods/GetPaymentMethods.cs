using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.PaymentMethods;

public class GetPaymentMethods
{
    public record Response(int Id, string Code, string? Name, string? Description, string? DigitalInvoiceCode);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/paymentmethods", HandlerAsync).WithTags(Tags.PaymentMethods);
        }
    }

    private static async Task<IResult> HandlerAsync(IPaymentMethodRepository repository)
    {
        var data = await repository.GetAllAsync();

        return ResultHelper.Ok(data.Select(MapResponse));
    }

    private static Response MapResponse(PaymentMethod e) =>
        new(e.Id, e.Code, e.Name, e.Description, e.DigitalInvoiceCode);
}
