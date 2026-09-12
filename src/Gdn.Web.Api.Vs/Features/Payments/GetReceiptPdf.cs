using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Payments.Reports;

namespace Gdn.Web.Api.Vs.Features.Payments;

/// <summary>
/// Generates and returns the PDF payment statement for a registered payment.
/// The document can be generated and re-generated at any time from the payment ID.
/// </summary>
public class GetReceiptPdf
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/payments/{id:int}/receipt-pdf", HandlerAsync).WithTags(Tags.Payments).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IPaymentRepository paymentRepository, ReceiptReportGenerator reportGenerator)
    {
        var payment = await paymentRepository.GetAsync(id);
        if (payment is null)
            return ResultHelper.NotFound(PaymentErrors.NotFound(id));

        var pdfBytes = await reportGenerator.GeneratePdfBytesAsync(id);
        var pdfStream = new MemoryStream(pdfBytes);
        return TypedResults.Stream(pdfStream, contentType: "application/octet-stream",
            fileDownloadName: $"distinta-pagamento-{id}.pdf");
    }
}
