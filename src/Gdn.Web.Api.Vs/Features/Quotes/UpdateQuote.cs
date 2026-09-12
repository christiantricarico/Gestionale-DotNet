using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Quotes;

public class UpdateQuote
{
    public record UpdateQuoteRowRequest(InputStatus InputStatus, long? Id, string? Description,
        decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, int? TaxRateId, int? ProductId);

    public record UpdateQuoteRequest(int Id, int Number, DateOnly Date, int CustomerId, IEnumerable<UpdateQuoteRowRequest> Rows);

    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId, int? ProductId);
    public record Response(int Id, int Number, DateOnly Date, int CustomerId, bool IsAccepted, DateTime? AcceptedAt, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/quotes", HandlerAsync).WithTags(Tags.Quotes).RequireAuthorization(Policies.AdminOnly);
        }
    }

    public sealed class Validator : AbstractValidator<UpdateQuoteRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty();
        }
    }

    private static async Task<IResult> HandlerAsync(UpdateQuoteRequest request, IValidator<UpdateQuoteRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var quoteRepository = unitOfWork.GetRepository<IQuoteRepository>();
        var quote = await quoteRepository.GetAsync(request.Id, ["Rows"]);
        if (quote is null)
            return ResultHelper.NotFound(QuoteErrors.NotFound(request.Id));

        if (quote.IsAccepted)
            return ResultHelper.Conflict(QuoteErrors.AlreadyAccepted(request.Id));

        var measurementUnitRepository = unitOfWork.GetRepository<IMeasurementUnitRepository>();
        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();

        var rowReferenceValidationError = await QuoteValidation.ValidateRowReferencesAsync(
            request.Rows.Select(r => r.MeasurementUnitId),
            request.Rows.Select(r => r.TaxRateId),
            measurementUnitRepository,
            taxRateRepository);

        if (rowReferenceValidationError is not null)
            return ResultHelper.BadRequest(rowReferenceValidationError);

        quote.Number = request.Number.ToString();
        quote.Date = request.Date;
        quote.CustomerId = request.CustomerId;

        ApplyRowChanges(quote, request.Rows);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(quote));
    }

    private static void ApplyRowChanges(Quote quote, IEnumerable<UpdateQuoteRowRequest> rows)
    {
        foreach (var requestRow in rows)
        {
            if (requestRow.InputStatus == InputStatus.Added)
            {
                quote.Rows.Add(MapRow(new QuoteRow(), requestRow));
                continue;
            }

            if (requestRow.InputStatus == InputStatus.Updated)
            {
                var row = quote.Rows.Single(r => r.Id == requestRow.Id);
                MapRow(row, requestRow);
                continue;
            }

            if (requestRow.InputStatus == InputStatus.Deleted && requestRow.Id.HasValue)
            {
                var row = quote.Rows.Single(r => r.Id == requestRow.Id);
                quote.Rows.Remove(row);
            }
        }
    }

    private static QuoteRow MapRow(QuoteRow row, UpdateQuoteRowRequest request)
    {
        row.RowType = ResolveRowType(request.ProductId);
        row.Description = request.Description;
        row.Quantity = request.Quantity;
        row.UnitPrice = request.UnitPrice;
        row.MeasurementUnitId = request.MeasurementUnitId;
        row.TaxRateId = request.TaxRateId;
        row.ProductId = request.ProductId;

        return row;
    }

    private static Response MapResponse(Quote quote)
        => new(quote.Id, int.Parse(quote.Number), quote.Date, quote.CustomerId, quote.IsAccepted, quote.AcceptedAt, quote.Rows.Select(MapResponseRow));

    private static ResponseRow MapResponseRow(QuoteRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId, row.ProductId);

    private static string ResolveRowType(int? productId)
        => productId.HasValue ? DocumentRowType.PRODUCT : DocumentRowType.DESCRIPTIVE;
}
