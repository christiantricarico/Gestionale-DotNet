using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Interventions;

public class UpdateIntervention
{
    public record UpdateInterventionRowRequest(InputStatus InputStatus, long? Id, string RowType, string? Description,
        decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, int? TaxRateId);

    public record UpdateInterventionRequest(int Id, int Number, DateOnly Date, int CustomerId, IEnumerable<UpdateInterventionRowRequest> Rows);

    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record Response(int Id, int Number, DateOnly Date, int CustomerId, bool IsInvoiced, int? InvoiceId, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/interventions", HandlerAsync).WithTags(Tags.Interventions);
        }
    }

    public sealed class Validator : AbstractValidator<UpdateInterventionRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty();
        }
    }

    private static async Task<IResult> HandlerAsync(UpdateInterventionRequest request, IValidator<UpdateInterventionRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var reportRepository = unitOfWork.GetRepository<IInterventionRepository>();
        var report = await reportRepository.GetAsync(request.Id, ["Rows"]);
        if (report is null)
            return ResultHelper.NotFound(InterventionErrors.NotFound(request.Id));

        if (report.IsInvoiced)
            return ResultHelper.Conflict(InterventionErrors.AlreadyInvoiced(request.Id));

        var measurementUnitRepository = unitOfWork.GetRepository<IMeasurementUnitRepository>();
        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();

        var rowReferenceValidationError = await InterventionValidation.ValidateRowReferencesAsync(
            request.Rows.Select(r => r.MeasurementUnitId),
            request.Rows.Select(r => r.TaxRateId),
            measurementUnitRepository,
            taxRateRepository);

        if (rowReferenceValidationError is not null)
            return ResultHelper.BadRequest(rowReferenceValidationError);

        report.Number = request.Number.ToString();
        report.Date = request.Date;
        report.CustomerId = request.CustomerId;

        ApplyRowChanges(report, request.Rows);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(MapResponse(report));
    }

    private static void ApplyRowChanges(Intervention report, IEnumerable<UpdateInterventionRowRequest> rows)
    {
        foreach (var requestRow in rows)
        {
            if (requestRow.InputStatus == InputStatus.Added)
            {
                report.Rows.Add(MapRow(new InterventionRow(), requestRow));
                continue;
            }

            if (requestRow.InputStatus == InputStatus.Updated)
            {
                var row = report.Rows.Single(r => r.Id == requestRow.Id);
                MapRow(row, requestRow);
                continue;
            }

            if (requestRow.InputStatus == InputStatus.Deleted && requestRow.Id.HasValue)
            {
                var row = report.Rows.Single(r => r.Id == requestRow.Id);
                report.Rows.Remove(row);
            }
        }
    }

    private static InterventionRow MapRow(InterventionRow row, UpdateInterventionRowRequest request)
    {
        row.Description = request.Description;
        row.Quantity = request.Quantity;
        row.UnitPrice = request.UnitPrice;
        row.MeasurementUnitId = request.MeasurementUnitId;
        row.TaxRateId = request.TaxRateId;

        return row;
    }

    private static Response MapResponse(Intervention report)
        => new(report.Id, int.Parse(report.Number), report.Date, report.CustomerId, report.IsInvoiced, report.InvoiceId, report.Rows.Select(MapResponseRow));

    private static ResponseRow MapResponseRow(InterventionRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);
}
