using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class DeleteCreditNote
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/creditnotes/{id:int}", HandlerAsync).WithTags(Tags.CreditNotes);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IUnitOfWork unitOfWork)
    {
        var creditNoteRepository = unitOfWork.GetRepository<ICreditNoteRepository>();

        var creditNote = await creditNoteRepository.GetAsync(id, ["Dues"]);
        if (creditNote is null)
            return ResultHelper.NotFound(CreditNoteErrors.NotFound(id));

        if (creditNote.Dues.Any(d => d.HasPayments))
            return ResultHelper.Conflict(CreditNoteErrors.HasPayments(id));

        await creditNoteRepository.RemoveAsync(id);
        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
