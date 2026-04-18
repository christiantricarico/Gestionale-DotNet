using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class UpdateInvoice
{
    public record UpdateInvoiceRowRequest(InputStatus InputStatus, long? Id, string RowType, string? Description,
        decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, int? TaxRateId);

    public record UpdateInvoiceDueRequest(InputStatus InputStatus, int? Id, DateOnly Date, decimal Amount);

    public record UpdateInvoiceRequest(int Id, int Number, DateOnly Date, int CustomerId,
        decimal? StampDutyAmount, bool StampDutyChargedToCustomer,
        IEnumerable<UpdateInvoiceRowRequest> Rows,
        IEnumerable<UpdateInvoiceDueRequest> Dues);

    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record ResponseDue(int Id, DateOnly Date, decimal Amount, decimal PaidAmount, bool IsPaid);
    public record Response(int Id, int Number, DateOnly Date, int CustomerId,
        decimal? StampDutyAmount, bool StampDutyChargedToCustomer,
        string PaymentStatus,
        IEnumerable<ResponseRow> Rows,
        IEnumerable<ResponseDue> Dues);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/invoices", Handler).WithTags(Tags.Invoices);
        }
    }

    public sealed class Validator : AbstractValidator<UpdateInvoiceRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty();
            RuleFor(e => e.StampDutyAmount).Equal(2.00m).When(e => e.StampDutyAmount.HasValue);
        }
    }

    private static async Task<IResult> Handler(UpdateInvoiceRequest request, IValidator<UpdateInvoiceRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var invoiceRepository = unitOfWork.GetRepository<IInvoiceRepository>();

        var invoice = await invoiceRepository.GetAsync(request.Id, ["Rows.TaxRate", "Dues.PaymentDues"]);
        if (invoice is null)
            return ResultHelper.NotFound(InvoiceErrors.NotFound(request.Id));

        var dueRepository = unitOfWork.GetRepository<IDueRepository>();

        invoice.Number = request.Number.ToString();
        invoice.Date = request.Date;
        invoice.CustomerId = request.CustomerId;
        invoice.StampDutyAmount = request.StampDutyAmount;
        invoice.StampDutyChargedToCustomer = request.StampDutyChargedToCustomer;

        ApplyRowChanges(invoice, request.Rows);

        var dueValidationError = await ApplyDueChangesAsync(invoice, request.Dues, dueRepository);
        if (dueValidationError is not null)
            return ResultHelper.BadRequest(dueValidationError);

        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();
        await PopulateMissingTaxRates(invoice, taxRateRepository);

        var hasDueChanges = request.Dues.Any(d => (int)d.InputStatus != 0);
        if (!hasDueChanges)
        {
            var reconcileError = await ReconcileDuesAsync(invoice, dueRepository);
            if (reconcileError is not null)
                return ResultHelper.BadRequest(reconcileError);
        }

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(invoice));
    }

    private static void ApplyRowChanges(Invoice invoice, IEnumerable<UpdateInvoiceRowRequest> rows)
    {
        foreach (var requestRow in rows)
        {
            if (requestRow.InputStatus == InputStatus.Added)
            {
                invoice.Rows.Add(MapInvoiceRow(new InvoiceRow(), requestRow));
                continue;
            }

            if (requestRow.InputStatus == InputStatus.Updated)
            {
                var row = invoice.Rows.Single(r => r.Id == requestRow.Id);
                MapInvoiceRow(row, requestRow);
                continue;
            }

            if (requestRow.InputStatus == InputStatus.Deleted && requestRow.Id.HasValue)
            {
                var row = invoice.Rows.Single(r => r.Id == requestRow.Id);
                invoice.Rows.Remove(row);
            }
        }
    }

    private static async Task<Error?> ApplyDueChangesAsync(Invoice invoice, IEnumerable<UpdateInvoiceDueRequest> dues, IDueRepository dueRepository)
    {
        foreach (var requestDue in dues)
        {
            if (requestDue.InputStatus == InputStatus.Added)
            {
                invoice.Dues.Add(new Due
                {
                    Date = requestDue.Date,
                    Amount = requestDue.Amount,
                    InvoiceId = invoice.Id,
                    CustomerId = invoice.CustomerId
                });
                continue;
            }

            if (requestDue.InputStatus == InputStatus.Updated && requestDue.Id.HasValue)
            {
                var due = invoice.Dues.Single(d => d.Id == requestDue.Id);
                due.Date = requestDue.Date;
                due.Amount = requestDue.Amount;
                continue;
            }

            if (requestDue.InputStatus == InputStatus.Deleted && requestDue.Id.HasValue)
            {
                var due = invoice.Dues.Single(d => d.Id == requestDue.Id);

                if (due.PaymentDues.Count > 0)
                    return new Error("Due:HasPayments", $"Cannot delete due {due.Id}: it has associated payments.");

                await DeleteDueAsync(invoice.Dues, due, dueRepository);
            }
        }

        return null;
    }

    private static async Task PopulateMissingTaxRates(Invoice invoice, ITaxRateRepository taxRateRepository)
    {
        var missingIds = invoice.Rows
            .Where(r => r.TaxRateId.HasValue && r.TaxRate is null)
            .Select(r => r.TaxRateId!.Value)
            .Distinct()
            .ToList();

        if (missingIds.Count == 0)
            return;

        var taxRates = (await taxRateRepository.GetAllAsync(r => missingIds.Contains(r.Id)))
            .ToDictionary(r => r.Id);

        foreach (var row in invoice.Rows.Where(r => r.TaxRateId.HasValue && r.TaxRate is null))
            row.TaxRate = taxRates.GetValueOrDefault(row.TaxRateId!.Value);
    }

    private static async Task<Error?> ReconcileDuesAsync(Invoice invoice, IDueRepository dueRepository)
    {
        var newTotal = InvoiceAmountCalculator.CalculateTotal(invoice);
        var currentDuesTotal = invoice.Dues.Sum(d => d.Amount);
        var delta = newTotal - currentDuesTotal;

        if (delta == 0m)
            return null;

        var orderedDues = invoice.Dues
            .OrderByDescending(d => d.Date)
            .ThenByDescending(d => d.Id)
            .ToList();

        if (delta > 0m)
        {
            var lastDue = orderedDues.FirstOrDefault();
            if (lastDue is null)
                return null;

            lastDue.Amount += delta;
            return null;
        }

        var toReduce = -delta;

        foreach (var due in orderedDues)
        {
            var reducible = due.Amount - due.PaidAmount;
            var actual = Math.Min(toReduce, reducible);

            due.Amount -= actual;
            toReduce -= actual;

            if (due.Amount == 0m)
                await DeleteDueAsync(invoice.Dues, due, dueRepository);

            if (toReduce == 0m)
                break;
        }

        if (toReduce > 0m)
            return InvoiceErrors.CannotReduceInvoiceAmount();

        return null;
    }

    private static async Task DeleteDueAsync(ICollection<Due> dues, Due due, IDueRepository dueRepository)
    {
        dues.Remove(due);
        await dueRepository.RemoveAsync(due.Id);
    }

    private static InvoiceRow MapInvoiceRow(InvoiceRow row, UpdateInvoiceRowRequest request)
    {
        row.Description = request.Description;
        row.Quantity = request.Quantity;
        row.UnitPrice = request.UnitPrice;
        row.MeasurementUnitId = request.MeasurementUnitId;
        row.TaxRateId = request.TaxRateId;

        return row;
    }

    private static Response MapResponse(Invoice invoice)
        => new(invoice.Id, int.Parse(invoice.Number), invoice.Date, invoice.CustomerId,
               invoice.StampDutyAmount, invoice.StampDutyChargedToCustomer,
               ResolvePaymentStatus(invoice),
               invoice.Rows.Select(MapResponseRow),
               invoice.Dues.Select(MapResponseDue));

    private static ResponseRow MapResponseRow(InvoiceRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);

    private static ResponseDue MapResponseDue(Due due)
        => new(due.Id, due.Date, due.Amount, due.PaidAmount, due.IsPaid);

    private static string ResolvePaymentStatus(Invoice invoice)
    {
        if (!invoice.Dues.Any())
            return PaymentStatus.NotPaid;

        if (invoice.IsPaid)
            return PaymentStatus.Paid;

        return invoice.Dues.Any(d => d.HasPayments)
            ? PaymentStatus.PartiallyPaid
            : PaymentStatus.NotPaid;
    }
}
