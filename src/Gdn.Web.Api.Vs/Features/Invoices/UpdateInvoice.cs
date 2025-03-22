using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class UpdateInvoice
{
    public record RequestRow(InputStatus InputStatus, long? Id, string RowType, string? Description,
        decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, int? TaxRateId);
    public record Request(int Id, string Number, DateOnly Date, int CustomerId, IEnumerable<RequestRow> Rows);

    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record Response(int Id, string Number, DateOnly Date, int CustomerId, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/invoices", Handler).WithTags(Tags.Invoices);
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

        var invoiceRepository = unitOfWork.GetRepository<IInvoiceRepository>();

        var invoice = await invoiceRepository.GetAsync(request.Id, ["Rows"]);
        if (invoice is null)
            return ResultHelper.NotFound(InvoiceErrors.NotFound(request.Id));

        MapInvoice(invoice, request);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(invoice));
    }

    private static void MapInvoice(Invoice invoice, Request request)
    {
        invoice.Number = request.Number;
        invoice.Date = request.Date;
        invoice.CustomerId = request.CustomerId;

        foreach (var requestRow in request.Rows)
        {
            if (requestRow.InputStatus == InputStatus.Added)
            {
                var row = new InvoiceRow();
                row = MapInvoiceRow(row, requestRow);
                invoice.Rows.Add(row);
            }

            if (requestRow.InputStatus == InputStatus.Updated)
            {
                var row = invoice.Rows.Single(r => r.Id == requestRow.Id);
                row = MapInvoiceRow(row, requestRow);
            }

            if (requestRow.InputStatus == InputStatus.Deleted)
            {
                var row = invoice.Rows.Single(r => r.Id == requestRow.Id);
                invoice.Rows.Remove(row);
            }
        }
    }

    private static InvoiceRow MapInvoiceRow(InvoiceRow row, RequestRow request)
    {
        row.Description = request.Description;
        row.Quantity = request.Quantity;
        row.UnitPrice = request.UnitPrice;
        row.MeasurementUnitId = request.MeasurementUnitId;
        row.TaxRateId = request.TaxRateId;

        return row;
    }

    private static Response MapResponse(Invoice invoice)
        => new(invoice.Id, invoice.Number, invoice.Date, invoice.CustomerId, invoice.Rows.Select(r => MapResponseRow(r)));

    private static ResponseRow MapResponseRow(InvoiceRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);
}
