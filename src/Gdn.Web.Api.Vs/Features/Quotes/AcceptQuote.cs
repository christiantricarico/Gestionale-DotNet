using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Quotes;

public class AcceptQuote
{
    public record Response(int Id, bool IsAccepted, DateTime? AcceptedAt);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/quotes/{id:int}/accept", HandlerAsync).WithTags(Tags.Quotes);
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

        quote.IsAccepted = true;
        quote.AcceptedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(new Response(quote.Id, quote.IsAccepted, quote.AcceptedAt));
    }
}
