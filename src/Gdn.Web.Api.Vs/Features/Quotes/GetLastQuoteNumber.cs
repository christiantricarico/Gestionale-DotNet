using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Quotes;

public class GetLastQuoteNumber
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/quotes/last-number", HandlerAsync).WithTags(Tags.Quotes);
        }
    }

    private static async Task<IResult> HandlerAsync(IQuoteRepository quoteRepository)
    {
        var quotes = await quoteRepository.GetAllAsync();
        var lastNumber = quotes
            .Select(q => int.TryParse(q.Number, out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        return ResultHelper.Ok(lastNumber);
    }
}
