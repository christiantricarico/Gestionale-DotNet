using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Quotes;

public class GetQuotes
{
    public record Response(int Id, string Number, DateOnly Date, int CustomerId, string? CustomerName, bool IsAccepted, DateTime? AcceptedAt, decimal TotalAmount);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/quotes", HandlerAsync).WithTags(Tags.Quotes);
        }
    }

    private static async Task<IResult> HandlerAsync(int? customerId, bool? isAccepted, IQuoteRepository quoteRepository)
    {
        var data = await quoteRepository.GetAllAsync(
            includes: ["Customer", "Rows.TaxRate"],
            predicate: quote => (!customerId.HasValue || quote.CustomerId == customerId.Value)
                && (!isAccepted.HasValue || quote.IsAccepted == isAccepted.Value));

        return ResultHelper.Ok(data.Select(MapResponse));
    }

    private static Response MapResponse(Quote quote)
        => new(
            quote.Id,
            quote.Number,
            quote.Date,
            quote.CustomerId,
            quote.Customer.Name,
            quote.IsAccepted,
            quote.AcceptedAt,
            QuoteAmountCalculator.CalculateTotal(quote));
}
