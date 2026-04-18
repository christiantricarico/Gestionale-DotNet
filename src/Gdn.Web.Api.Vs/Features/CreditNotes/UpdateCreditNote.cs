using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Invoices;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class UpdateCreditNote
{
    public record UpdateCreditNoteRowRequest(InputStatus InputStatus, long? Id, string RowType, string? Description,
        decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, int? TaxRateId);

    public record UpdateCreditNoteDueRequest(InputStatus InputStatus, int? Id, DateOnly Date, decimal Amount);

    public record UpdateCreditNoteRequest(int Id, int Number, DateOnly Date, int CustomerId,
        decimal? StampDutyAmount, bool StampDutyChargedToCustomer,
        IEnumerable<UpdateCreditNoteRowRequest> Rows,
        IEnumerable<UpdateCreditNoteDueRequest> Dues);

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
            app.MapPut("api/creditnotes", HandlerAsync).WithTags(Tags.CreditNotes);
        }
    }

    public sealed class Validator : AbstractValidator<UpdateCreditNoteRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty();
            RuleFor(e => e.StampDutyAmount).Equal(2.00m).When(e => e.StampDutyAmount.HasValue);
        }
    }

    private static async Task<IResult> HandlerAsync(UpdateCreditNoteRequest request, IValidator<UpdateCreditNoteRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var creditNoteRepository = unitOfWork.GetRepository<ICreditNoteRepository>();

        var creditNote = await creditNoteRepository.GetAsync(request.Id, ["Rows.TaxRate", "Dues.PaymentDues"]);
        if (creditNote is null)
            return ResultHelper.NotFound(CreditNoteErrors.NotFound(request.Id));

        var dueRepository = unitOfWork.GetRepository<IDueRepository>();

        creditNote.Number = request.Number.ToString();
        creditNote.Date = request.Date;
        creditNote.CustomerId = request.CustomerId;
        creditNote.StampDutyAmount = request.StampDutyAmount;
        creditNote.StampDutyChargedToCustomer = request.StampDutyChargedToCustomer;

        ApplyRowChanges(creditNote, request.Rows);

        var dueValidationError = await ApplyDueChangesAsync(creditNote, request.Dues, dueRepository);
        if (dueValidationError is not null)
            return ResultHelper.BadRequest(dueValidationError);

        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();
        await PopulateMissingTaxRates(creditNote, taxRateRepository);

        var hasDueChanges = request.Dues.Any(d => (int)d.InputStatus != 0);
        if (!hasDueChanges)
        {
            var reconcileError = await ReconcileDuesAsync(creditNote, dueRepository);
            if (reconcileError is not null)
                return ResultHelper.BadRequest(reconcileError);
        }

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(creditNote));
    }

    private static void ApplyRowChanges(CreditNote creditNote, IEnumerable<UpdateCreditNoteRowRequest> rows)
    {
        foreach (var requestRow in rows)
        {
            if (requestRow.InputStatus == InputStatus.Added)
            {
                creditNote.Rows.Add(MapCreditNoteRow(new CreditNoteRow(), requestRow));
                continue;
            }

            if (requestRow.InputStatus == InputStatus.Updated)
            {
                var row = creditNote.Rows.Single(r => r.Id == requestRow.Id);
                MapCreditNoteRow(row, requestRow);
                continue;
            }

            if (requestRow.InputStatus == InputStatus.Deleted && requestRow.Id.HasValue)
            {
                var row = creditNote.Rows.Single(r => r.Id == requestRow.Id);
                creditNote.Rows.Remove(row);
            }
        }
    }

    private static async Task<Error?> ApplyDueChangesAsync(CreditNote creditNote, IEnumerable<UpdateCreditNoteDueRequest> dues, IDueRepository dueRepository)
    {
        foreach (var requestDue in dues)
        {
            if (requestDue.InputStatus == InputStatus.Added)
            {
                creditNote.Dues.Add(new Due
                {
                    Date = requestDue.Date,
                    Amount = ToSignedCreditNoteAmount(requestDue.Amount),
                    CreditNoteId = creditNote.Id,
                    CustomerId = creditNote.CustomerId
                });
                continue;
            }

            if (requestDue.InputStatus == InputStatus.Updated && requestDue.Id.HasValue)
            {
                var due = creditNote.Dues.Single(d => d.Id == requestDue.Id);
                due.Date = requestDue.Date;
                due.Amount = ToSignedCreditNoteAmount(requestDue.Amount);
                continue;
            }

            if (requestDue.InputStatus == InputStatus.Deleted && requestDue.Id.HasValue)
            {
                var due = creditNote.Dues.Single(d => d.Id == requestDue.Id);

                if (due.PaymentDues.Count > 0)
                    return new Error("Due:HasPayments", $"Cannot delete due {due.Id}: it has associated payments.");

                await DeleteDueAsync(creditNote.Dues, due, dueRepository);
            }
        }

        return null;
    }

    private static async Task PopulateMissingTaxRates(CreditNote creditNote, ITaxRateRepository taxRateRepository)
    {
        var missingIds = creditNote.Rows
            .Where(r => r.TaxRateId.HasValue && r.TaxRate is null)
            .Select(r => r.TaxRateId!.Value)
            .Distinct()
            .ToList();

        if (missingIds.Count == 0)
            return;

        var taxRates = (await taxRateRepository.GetAllAsync(r => missingIds.Contains(r.Id)))
            .ToDictionary(r => r.Id);

        foreach (var row in creditNote.Rows.Where(r => r.TaxRateId.HasValue && r.TaxRate is null))
            row.TaxRate = taxRates.GetValueOrDefault(row.TaxRateId!.Value);
    }

    private static async Task<Error?> ReconcileDuesAsync(CreditNote creditNote, IDueRepository dueRepository)
    {
        var newTotal = ToSignedCreditNoteAmount(CreditNoteAmountCalculator.CalculateTotal(creditNote));
        var currentDuesTotal = creditNote.Dues.Sum(d => d.Amount);
        var delta = newTotal - currentDuesTotal;

        if (delta == 0m)
            return null;

        var orderedDues = creditNote.Dues
            .OrderByDescending(d => d.Date)
            .ThenByDescending(d => d.Id)
            .ToList();

        if (delta < 0m)
        {
            var lastDue = orderedDues.FirstOrDefault();
            if (lastDue is null)
                return null;

            lastDue.Amount += delta;
            return null;
        }

        var toReduce = delta;

        foreach (var due in orderedDues)
        {
            var reducible = Math.Abs(due.RemainingAmount);
            var actual = Math.Min(toReduce, reducible);

            due.Amount += actual;
            toReduce -= actual;

            if (due.Amount == 0m)
                await DeleteDueAsync(creditNote.Dues, due, dueRepository);

            if (toReduce == 0m)
                break;
        }

        if (toReduce > 0m)
            return CreditNoteErrors.CannotReduceCreditNoteAmount();

        return null;
    }

    private static async Task DeleteDueAsync(ICollection<Due> dues, Due due, IDueRepository dueRepository)
    {
        dues.Remove(due);
        await dueRepository.RemoveAsync(due.Id);
    }

    private static CreditNoteRow MapCreditNoteRow(CreditNoteRow row, UpdateCreditNoteRowRequest request)
    {
        row.Description = request.Description;
        row.Quantity = request.Quantity;
        row.UnitPrice = request.UnitPrice;
        row.MeasurementUnitId = request.MeasurementUnitId;
        row.TaxRateId = request.TaxRateId;

        return row;
    }

    private static Response MapResponse(CreditNote creditNote)
        => new(creditNote.Id, int.Parse(creditNote.Number), creditNote.Date, creditNote.CustomerId,
               creditNote.StampDutyAmount, creditNote.StampDutyChargedToCustomer,
               ResolvePaymentStatus(creditNote),
               creditNote.Rows.Select(MapResponseRow),
               creditNote.Dues.Select(MapResponseDue));

    private static ResponseRow MapResponseRow(CreditNoteRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);

    private static ResponseDue MapResponseDue(Due due)
        => new(due.Id, due.Date, due.Amount, due.PaidAmount, due.IsPaid);

    private static string ResolvePaymentStatus(CreditNote creditNote)
    {
        if (!creditNote.Dues.Any())
            return PaymentStatus.NotPaid;

        if (creditNote.IsPaid)
            return PaymentStatus.Paid;

        return creditNote.Dues.Any(d => d.HasPayments)
            ? PaymentStatus.PartiallyPaid
            : PaymentStatus.NotPaid;
    }

    private static decimal ToSignedCreditNoteAmount(decimal amount) => amount > 0m ? -amount : amount;
}
