using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class DeleteInvoice
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/invoices/{id:int}", HandlerAsync).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IUnitOfWork unitOfWork)
    {
        var invoiceRepository = unitOfWork.GetRepository<IInvoiceRepository>();

        // Load with Dues so EF Core can cascade the in-memory delete, and to validate
        // that no due already has recorded payments before attempting deletion.
        var invoice = await invoiceRepository.GetAsync(id, ["Dues"]);
        if (invoice is null)
            return ResultHelper.NotFound(InvoiceErrors.NotFound(id));

        if (invoice.Dues.Any(d => d.HasPayments))
            return ResultHelper.Conflict(InvoiceErrors.HasPayments(id));

        await invoiceRepository.RemoveAsync(id);
        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
