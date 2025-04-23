using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class CreateInvoice
{
    public record RequestRow(string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record Request(string Number, DateOnly Date, int CustomerId, IEnumerable<RequestRow> Rows);

    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record Response(int Id, string Number, DateOnly Date, int CustomerId, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/invoices", Handler).WithTags(Tags.Invoices);
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty().MaximumLength(50);
        }
    }

    private static async Task<IResult> Handler(Request request, IValidator<Request> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var invoice = MapInvoice(request);

        var invoiceRepository = unitOfWork.GetRepository<IInvoiceRepository>();
        invoiceRepository.Add(invoice);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(MapResponse(invoice));
    }

    private static Invoice MapInvoice(Request request) => new()
    {
        Number = request.Number,
        Date = request.Date,
        CustomerId = request.CustomerId,
        Rows = request.Rows.Select(r => MapInvoiceRow(r)).ToList()
    };

    private static InvoiceRow MapInvoiceRow(RequestRow request) => new()
    {
        RowType = DocumentRowType.DESCRIPTIVE,
        Description = request.Description,
        Quantity = request.Quantity,
        UnitPrice = request.UnitPrice,
        MeasurementUnitId = request.MeasurementUnitId,
        TaxRateId = request.TaxRateId
    };

    private static Response MapResponse(Invoice invoice)
        => new(invoice.Id, invoice.Number, invoice.Date, invoice.CustomerId, invoice.Rows.Select(r => MapResponseRow(r)));

    private static ResponseRow MapResponseRow(InvoiceRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);
}
