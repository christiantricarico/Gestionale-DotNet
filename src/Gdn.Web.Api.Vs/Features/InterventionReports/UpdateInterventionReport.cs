using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.InterventionReports;

public class UpdateInterventionReport
{
    public record UpdateInterventionReportRowRequest(InputStatus InputStatus, long? Id, string RowType, string? Description,
        decimal? Quantity, decimal? UnitPrice,
        int? MeasurementUnitId, int? TaxRateId);

    public record UpdateInterventionReportRequest(int Id, int Number, DateOnly Date, int CustomerId, IEnumerable<UpdateInterventionReportRowRequest> Rows);

    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record Response(int Id, int Number, DateOnly Date, int CustomerId, bool IsInvoiced, int? InvoiceId, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/interventionreports", HandlerAsync).WithTags(Tags.InterventionReports);
        }
    }

    public sealed class Validator : AbstractValidator<UpdateInterventionReportRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty();
        }
    }

    private static async Task<IResult> HandlerAsync(UpdateInterventionReportRequest request, IValidator<UpdateInterventionReportRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var reportRepository = unitOfWork.GetRepository<IInterventionReportRepository>();
        var report = await reportRepository.GetAsync(request.Id, ["Rows"]);
        if (report is null)
            return ResultHelper.NotFound(InterventionReportErrors.NotFound(request.Id));

        if (report.IsInvoiced)
            return ResultHelper.Conflict(InterventionReportErrors.AlreadyInvoiced(request.Id));

        var measurementUnitRepository = unitOfWork.GetRepository<IMeasurementUnitRepository>();
        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();

        var rowReferenceValidationError = await InterventionReportValidation.ValidateRowReferencesAsync(
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

    private static void ApplyRowChanges(InterventionReport report, IEnumerable<UpdateInterventionReportRowRequest> rows)
    {
        foreach (var requestRow in rows)
        {
            if (requestRow.InputStatus == InputStatus.Added)
            {
                report.Rows.Add(MapRow(new InterventionReportRow(), requestRow));
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

    private static InterventionReportRow MapRow(InterventionReportRow row, UpdateInterventionReportRowRequest request)
    {
        row.Description = request.Description;
        row.Quantity = request.Quantity;
        row.UnitPrice = request.UnitPrice;
        row.MeasurementUnitId = request.MeasurementUnitId;
        row.TaxRateId = request.TaxRateId;

        return row;
    }

    private static Response MapResponse(InterventionReport report)
        => new(report.Id, int.Parse(report.Number), report.Date, report.CustomerId, report.IsInvoiced, report.InvoiceId, report.Rows.Select(MapResponseRow));

    private static ResponseRow MapResponseRow(InterventionReportRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);
}
