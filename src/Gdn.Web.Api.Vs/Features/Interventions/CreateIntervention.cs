using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Interventions;

public class CreateIntervention
{
    public record CreateInterventionRowRequest(string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId, int? ProductId);
    public record CreateInterventionRequest(int Number, DateOnly Date, int CustomerId, IEnumerable<CreateInterventionRowRequest> Rows);

    public record ResponseRow(long Id, string RowType, string? Description, decimal? Quantity, decimal? UnitPrice, int? MeasurementUnitId, int? TaxRateId, int? ProductId);
    public record Response(int Id, int Number, DateOnly Date, int CustomerId, bool IsInvoiced, int? InvoiceId, IEnumerable<ResponseRow> Rows);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/interventions", HandlerAsync).WithTags(Tags.Interventions);
        }
    }

    public sealed class Validator : AbstractValidator<CreateInterventionRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Number).NotEmpty();
        }
    }

    private static async Task<IResult> HandlerAsync(CreateInterventionRequest request, IValidator<CreateInterventionRequest> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var measurementUnitRepository = unitOfWork.GetRepository<IMeasurementUnitRepository>();
        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();

        var rowReferenceValidationError = await InterventionValidation.ValidateRowReferencesAsync(
            request.Rows.Select(r => r.MeasurementUnitId),
            request.Rows.Select(r => r.TaxRateId),
            measurementUnitRepository,
            taxRateRepository);

        if (rowReferenceValidationError is not null)
            return ResultHelper.BadRequest(rowReferenceValidationError);

        var report = MapReport(request);

        var reportRepository = unitOfWork.GetRepository<IInterventionRepository>();
        reportRepository.Add(report);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(MapResponse(report));
    }

    private static Intervention MapReport(CreateInterventionRequest request) => new()
    {
        Number = request.Number.ToString(),
        Date = request.Date,
        CustomerId = request.CustomerId,
        IsInvoiced = false,
        Rows = request.Rows.Select(MapRow).ToList()
    };

    private static InterventionRow MapRow(CreateInterventionRowRequest request) => new()
    {
        RowType = ResolveRowType(request.ProductId),
        Description = request.Description,
        Quantity = request.Quantity,
        UnitPrice = request.UnitPrice,
        MeasurementUnitId = request.MeasurementUnitId,
        TaxRateId = request.TaxRateId,
        ProductId = request.ProductId
    };

    private static Response MapResponse(Intervention report)
        => new(report.Id, int.Parse(report.Number), report.Date, report.CustomerId, report.IsInvoiced, report.InvoiceId, report.Rows.Select(MapResponseRow));

    private static ResponseRow MapResponseRow(InterventionRow row)
        => new(row.Id, row.RowType, row.Description, row.Quantity, row.UnitPrice, row.MeasurementUnitId, row.TaxRateId, row.ProductId);

    private static string ResolveRowType(int? productId)
        => productId.HasValue ? DocumentRowType.PRODUCT : DocumentRowType.DESCRIPTIVE;
}
