using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.PaymentMethods;

public class GetPaymentMethodById
{
    public record Response(int Id, string Code, string? Name, string? Description, string? DigitalInvoiceCode);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/paymentmethods/{id:int}", Handler).WithTags(Tags.PaymentMethods);
        }
    }

    private static async Task<IResult> Handler(int id, IPaymentMethodRepository repository)
    {
        var entity = await repository.GetAsync(id);

        return entity is not null
            ? ResultHelper.Ok(MapResponse(entity))
            : ResultHelper.NotFound();
    }

    private static Response MapResponse(PaymentMethod e) =>
        new(e.Id, e.Code, e.Name, e.Description, e.DigitalInvoiceCode);
}
