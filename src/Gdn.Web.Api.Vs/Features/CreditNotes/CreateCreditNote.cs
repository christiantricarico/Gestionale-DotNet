using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class CreateCreditNote
{
    public record CreateCreditNoteRowRequest(string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record CreateCreditNoteRequest(int Number, DateOnly Date, int CustomerId, decimal? StampDutyAmount, bool StampDutyChargedToCustomer, IEnumerable<CreateCreditNoteRowRequest> Rows);

    public record ResponseDue(int Id, DateOnly Date, decimal Amount, decimal PaidAmount, bool IsPaid);
    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record Response(int Id, int Number, DateOnly Date, int CustomerId, decimal? StampDutyAmount, bool StampDutyChargedToCustomer, IEnumerable<ResponseRow> Rows, IEnumerable<ResponseDue> Dues);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/creditnotes", Handler).WithTags(Tags.CreditNotes);
        }
    }

    public sealed class Validator : AbstractValidator<CreateCreditNoteRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty();
            RuleFor(e => e.StampDutyAmount).Equal(2.00m).When(e => e.StampDutyAmount.HasValue);
        }
    }

    private static async Task<IResult> Handler(CreateCreditNoteRequest request, IValidator<CreateCreditNoteRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var creditNote = MapCreditNote(request);

        var creditNoteRepository = unitOfWork.GetRepository<ICreditNoteRepository>();
        creditNoteRepository.Add(creditNote);

        await unitOfWork.SaveChangesAsync();

        var creditNoteWithDetails = await creditNoteRepository.GetAsync(creditNote.Id, ["Rows.TaxRate"]);
        decimal totalAmount = CreditNoteAmountCalculator.CalculateTotal(creditNoteWithDetails!);

        var due = new Due
        {
            Date = creditNote.Date,
            Amount = totalAmount,
            CreditNoteId = creditNote.Id,
            CustomerId = creditNote.CustomerId
        };

        var dueRepository = unitOfWork.GetRepository<IDueRepository>();
        dueRepository.Add(due);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(MapResponse(creditNoteWithDetails!, [due]));
    }

    private static CreditNote MapCreditNote(CreateCreditNoteRequest request) => new()
    {
        Number = request.Number.ToString(),
        Date = request.Date,
        CustomerId = request.CustomerId,
        StampDutyAmount = request.StampDutyAmount,
        StampDutyChargedToCustomer = request.StampDutyChargedToCustomer,
        Rows = request.Rows.Select(MapCreditNoteRow).ToList()
    };

    private static CreditNoteRow MapCreditNoteRow(CreateCreditNoteRowRequest request) => new()
    {
        RowType = DocumentRowType.DESCRIPTIVE,
        Description = request.Description,
        Quantity = request.Quantity,
        UnitPrice = request.UnitPrice,
        MeasurementUnitId = request.MeasurementUnitId,
        TaxRateId = request.TaxRateId
    };

    private static Response MapResponse(CreditNote creditNote, IEnumerable<Due> dues)
        => new(creditNote.Id, int.Parse(creditNote.Number), creditNote.Date, creditNote.CustomerId,
               creditNote.StampDutyAmount, creditNote.StampDutyChargedToCustomer,
               creditNote.Rows.Select(MapResponseRow),
               dues.Select(MapResponseDue));

    private static ResponseRow MapResponseRow(CreditNoteRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);

    private static ResponseDue MapResponseDue(Due due)
        => new(due.Id, due.Date, due.Amount, due.PaidAmount, due.IsPaid);
}
