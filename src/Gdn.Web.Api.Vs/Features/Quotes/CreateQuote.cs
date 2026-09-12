using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Quotes;

public class CreateQuote
{
    public record CreateQuoteRowRequest(string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId, int? ProductId);
    public record CreateQuoteRequest(int Number, DateOnly Date, int CustomerId, IEnumerable<CreateQuoteRowRequest> Rows);

    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId, int? ProductId);
    public record Response(int Id, int Number, DateOnly Date, int CustomerId, bool IsAccepted, DateTime? AcceptedAt, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/quotes", HandlerAsync).WithTags(Tags.Quotes).RequireAuthorization(Policies.AdminOnly);
        }
    }

    public sealed class Validator : AbstractValidator<CreateQuoteRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty();
        }
    }

    private static async Task<IResult> HandlerAsync(CreateQuoteRequest request, IValidator<CreateQuoteRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var measurementUnitRepository = unitOfWork.GetRepository<IMeasurementUnitRepository>();
        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();

        var rowReferenceValidationError = await QuoteValidation.ValidateRowReferencesAsync(
            request.Rows.Select(r => r.MeasurementUnitId),
            request.Rows.Select(r => r.TaxRateId),
            measurementUnitRepository,
            taxRateRepository);

        if (rowReferenceValidationError is not null)
            return ResultHelper.BadRequest(rowReferenceValidationError);

        var quote = MapQuote(request);

        var quoteRepository = unitOfWork.GetRepository<IQuoteRepository>();
        quoteRepository.Add(quote);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(MapResponse(quote));
    }

    private static Quote MapQuote(CreateQuoteRequest request) => new()
    {
        Number = request.Number.ToString(),
        Date = request.Date,
        CustomerId = request.CustomerId,
        IsAccepted = false,
        Rows = request.Rows.Select(MapRow).ToList()
    };

    private static QuoteRow MapRow(CreateQuoteRowRequest request) => new()
    {
        RowType = ResolveRowType(request.ProductId),
        Description = request.Description,
        Quantity = request.Quantity,
        UnitPrice = request.UnitPrice,
        MeasurementUnitId = request.MeasurementUnitId,
        TaxRateId = request.TaxRateId,
        ProductId = request.ProductId
    };

    private static Response MapResponse(Quote quote)
        => new(quote.Id, int.Parse(quote.Number), quote.Date, quote.CustomerId, quote.IsAccepted, quote.AcceptedAt, quote.Rows.Select(MapResponseRow));

    private static ResponseRow MapResponseRow(QuoteRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId, row.ProductId);

    private static string ResolveRowType(int? productId)
        => productId.HasValue ? DocumentRowType.PRODUCT : DocumentRowType.DESCRIPTIVE;
}
