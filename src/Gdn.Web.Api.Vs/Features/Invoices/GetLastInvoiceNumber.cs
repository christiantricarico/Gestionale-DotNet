using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class GetLastInvoiceNumber
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/invoices/last-number", Handler).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> Handler(IInvoiceRepository invoiceRepository)
    {
        // Recupera tutte le fatture, ordina per numero decrescente (convertito a int), prendi la prima
        var invoices = await invoiceRepository.GetAllAsync();
        var lastNumber = invoices
            .Select(i => int.TryParse(i.Number, out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        return ResultHelper.Ok(lastNumber);
    }
}