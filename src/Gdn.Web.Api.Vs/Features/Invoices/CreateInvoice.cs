using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Interventions;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class CreateInvoice
{
    public record CreateInvoiceRowRequest(string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId, int? ProductId, string? RowType);
    public record CreateInvoiceRequest(int Number, DateOnly Date, int CustomerId, decimal? StampDutyAmount, bool StampDutyChargedToCustomer, IEnumerable<CreateInvoiceRowRequest> Rows, IEnumerable<int>? InterventionIds);

    public record ResponseDue(int Id, DateOnly Date, decimal Amount, decimal PaidAmount, bool IsPaid);
    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId, int? ProductId);
    public record Response(int Id, int Number, DateOnly Date, int CustomerId, decimal? StampDutyAmount, bool StampDutyChargedToCustomer, IEnumerable<ResponseRow> Rows, IEnumerable<ResponseDue> Dues, IEnumerable<int> InterventionIds);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/invoices", HandlerAsync).WithTags(Tags.Invoices);
        }
    }

    public sealed class Validator : AbstractValidator<CreateInvoiceRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty();
            RuleFor(e => e.StampDutyAmount).Equal(2.00m).When(e => e.StampDutyAmount.HasValue);
        }
    }

    private static async Task<IResult> HandlerAsync(CreateInvoiceRequest request, IValidator<CreateInvoiceRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var invoice = MapInvoice(request);

        var invoiceRepository = unitOfWork.GetRepository<IInvoiceRepository>();
        var interventionReportRepository = unitOfWork.GetRepository<IInterventionRepository>();

        var interventionReportIds = request.InterventionIds?.Distinct().ToList() ?? new List<int>();
        var interventionReports = interventionReportIds.Count == 0
            ? new List<Intervention>()
            : (await interventionReportRepository.GetAllAsync(r => interventionReportIds.Contains(r.Id), ["Rows"])).ToList();

        if (interventionReports.Count != interventionReportIds.Count)
        {
            var missingId = interventionReportIds.First(id => interventionReports.All(r => r.Id != id));
            return ResultHelper.NotFound(InterventionErrors.NotFound(missingId));
        }

        foreach (var report in interventionReports)
        {
            if (report.IsInvoiced)
                return ResultHelper.Conflict(InterventionErrors.AlreadyInvoiced(report.Id));

            if (report.CustomerId != request.CustomerId)
                return ResultHelper.BadRequest(InterventionErrors.InvalidCustomer(report.Id));

            if (!report.Rows.Any())
                return ResultHelper.BadRequest(InterventionErrors.EmptyRows(report.Id));

            foreach (var reportRow in report.Rows)
                invoice.Rows.Add(MapInvoiceRow(reportRow));
        }

        invoiceRepository.Add(invoice);

        await unitOfWork.SaveChangesAsync();

        foreach (var report in interventionReports)
        {
            report.IsInvoiced = true;
            report.InvoiceId = invoice.Id;
        }

        // Reload with tax rates to compute the total for the auto-generated due.
        var invoiceWithDetails = await invoiceRepository.GetAsync(invoice.Id, ["Rows.TaxRate"]);
        decimal totalAmount = InvoiceAmountCalculator.CalculateTotal(invoiceWithDetails!);

        var due = new Due
        {
            Date = invoice.Date,
            Amount = totalAmount,
            InvoiceId = invoice.Id,
            CustomerId = invoice.CustomerId
        };

        var dueRepository = unitOfWork.GetRepository<IDueRepository>();
        dueRepository.Add(due);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(MapResponse(invoiceWithDetails!, [due], interventionReports.Select(r => r.Id)));
    }

    private static Invoice MapInvoice(CreateInvoiceRequest request) => new()
    {
        Number = request.Number.ToString(),
        Date = request.Date,
        CustomerId = request.CustomerId,
        StampDutyAmount = request.StampDutyAmount,
        StampDutyChargedToCustomer = request.StampDutyChargedToCustomer,
        Rows = request.Rows.Select(MapInvoiceRow).ToList()
    };

    private static InvoiceRow MapInvoiceRow(CreateInvoiceRowRequest request) => new()
    {
        RowType = request.RowType ?? DocumentRowType.DESCRIPTIVE,
        Description = request.Description,
        Quantity = request.Quantity,
        UnitPrice = request.UnitPrice,
        MeasurementUnitId = request.MeasurementUnitId,
        TaxRateId = request.TaxRateId,
        ProductId = request.ProductId
    };

    private static InvoiceRow MapInvoiceRow(InterventionRow reportRow) => new()
    {
        RowType = reportRow.RowType,
        Description = reportRow.Description,
        Quantity = reportRow.Quantity,
        UnitPrice = reportRow.UnitPrice,
        MeasurementUnitId = reportRow.MeasurementUnitId,
        TaxRateId = reportRow.TaxRateId,
        ProductId = reportRow.ProductId
    };

    private static Response MapResponse(Invoice invoice, IEnumerable<Due> dues, IEnumerable<int> interventionReportIds)
        => new(invoice.Id, int.Parse(invoice.Number), invoice.Date, invoice.CustomerId,
               invoice.StampDutyAmount, invoice.StampDutyChargedToCustomer,
                invoice.Rows.Select(MapResponseRow),
               dues.Select(MapResponseDue),
               interventionReportIds);

    private static ResponseRow MapResponseRow(InvoiceRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId, row.ProductId);

    private static ResponseDue MapResponseDue(Due due)
        => new(due.Id, due.Date, due.Amount, due.PaidAmount, due.IsPaid);
}
