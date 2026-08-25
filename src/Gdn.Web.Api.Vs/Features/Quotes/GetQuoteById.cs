using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Quotes;

public class GetQuoteById
{
    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, string? MeasurementUnitCode, string? MeasurementUnitName,
        int? TaxRateId, string? TaxRateName, decimal? TaxRateValue, int? ProductId);

    public record Response(int Id, string Number, DateOnly Date, int CustomerId, bool IsAccepted, DateTime? AcceptedAt, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/quotes/{id:int}", HandlerAsync).WithTags(Tags.Quotes);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IQuoteRepository quoteRepository)
    {
        var data = await quoteRepository.GetAsync(id, ["Rows.TaxRate", "Rows.MeasurementUnit", "Rows.Product"]);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound(QuoteErrors.NotFound(id));
    }

    private static Response MapResponse(Quote quote)
        => new(quote.Id, quote.Number, quote.Date, quote.CustomerId, quote.IsAccepted, quote.AcceptedAt, quote.Rows.Select(MapResponseRow));

    private static ResponseRow MapResponseRow(QuoteRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice,
               row.MeasurementUnitId, row.MeasurementUnit?.Code, row.MeasurementUnit?.Name,
               row.TaxRateId,
               string.IsNullOrWhiteSpace(row.TaxRate?.Name) ? row.TaxRate?.Code : row.TaxRate?.Name,
               row.TaxRate?.Rate, row.ProductId);
}
