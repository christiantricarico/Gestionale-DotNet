using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class GetLastCreditNoteNumber
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/creditnotes/last-number", HandlerAsync).WithTags(Tags.CreditNotes).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(ICreditNoteRepository creditNoteRepository)
    {
        var creditNotes = await creditNoteRepository.GetAllAsync();
        var lastNumber = creditNotes
            .Select(cn => int.TryParse(cn.Number, out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        return ResultHelper.Ok(lastNumber);
    }
}
