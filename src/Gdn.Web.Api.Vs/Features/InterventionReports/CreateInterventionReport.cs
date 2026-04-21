using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.InterventionReports;

public class CreateInterventionReport
{
    public record CreateInterventionReportRowRequest(string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record CreateInterventionReportRequest(int Number, DateOnly Date, int CustomerId, IEnumerable<CreateInterventionReportRowRequest> Rows);

    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId);
    public record Response(int Id, int Number, DateOnly Date, int CustomerId, bool IsInvoiced, int? InvoiceId, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/interventionreports", HandlerAsync).WithTags(Tags.InterventionReports);
        }
    }

    public sealed class Validator : AbstractValidator<CreateInterventionReportRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty();
        }
    }

    private static async Task<IResult> HandlerAsync(CreateInterventionReportRequest request, IValidator<CreateInterventionReportRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var measurementUnitRepository = unitOfWork.GetRepository<IMeasurementUnitRepository>();
        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();

        var rowReferenceValidationError = await InterventionReportValidation.ValidateRowReferencesAsync(
            request.Rows.Select(r => r.MeasurementUnitId),
            request.Rows.Select(r => r.TaxRateId),
            measurementUnitRepository,
            taxRateRepository);

        if (rowReferenceValidationError is not null)
            return ResultHelper.BadRequest(rowReferenceValidationError);

        var report = MapReport(request);

        var reportRepository = unitOfWork.GetRepository<IInterventionReportRepository>();
        reportRepository.Add(report);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(MapResponse(report));
    }

    private static InterventionReport MapReport(CreateInterventionReportRequest request) => new()
    {
        Number = request.Number.ToString(),
        Date = request.Date,
        CustomerId = request.CustomerId,
        IsInvoiced = false,
        Rows = request.Rows.Select(MapRow).ToList()
    };

    private static InterventionReportRow MapRow(CreateInterventionReportRowRequest request) => new()
    {
        RowType = DocumentRowType.DESCRIPTIVE,
        Description = request.Description,
        Quantity = request.Quantity,
        UnitPrice = request.UnitPrice,
        MeasurementUnitId = request.MeasurementUnitId,
        TaxRateId = request.TaxRateId
    };

    private static Response MapResponse(InterventionReport report)
        => new(report.Id, int.Parse(report.Number), report.Date, report.CustomerId, report.IsInvoiced, report.InvoiceId, report.Rows.Select(MapResponseRow));

    private static ResponseRow MapResponseRow(InterventionReportRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId);
}
