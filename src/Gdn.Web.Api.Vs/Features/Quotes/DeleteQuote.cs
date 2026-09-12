using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Quotes;

public class DeleteQuote
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/quotes/{id:int}", HandlerAsync).WithTags(Tags.Quotes).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IUnitOfWork unitOfWork)
    {
        var quoteRepository = unitOfWork.GetRepository<IQuoteRepository>();
        var quote = await quoteRepository.GetAsync(id);
        if (quote is null)
            return ResultHelper.NotFound(QuoteErrors.NotFound(id));

        if (quote.IsAccepted)
            return ResultHelper.Conflict(QuoteErrors.AlreadyAccepted(id));

        await quoteRepository.RemoveAsync(id);
        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
