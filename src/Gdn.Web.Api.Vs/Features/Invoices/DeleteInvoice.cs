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
            app.MapDelete("api/invoices/{id:int}", Handler).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> Handler(int id, IUnitOfWork unitOfWork)
    {
        var invoiceRepository = unitOfWork.GetRepository<IInvoiceRepository>();
        await invoiceRepository.RemoveAsync(id);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
