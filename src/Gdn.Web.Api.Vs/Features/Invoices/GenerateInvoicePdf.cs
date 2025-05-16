using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Invoices.Reports;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class GenerateInvoicePdf
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/invoices/pdf/{id}", Handler).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> Handler(int id, InvoiceReportGenerator reportGenerator)
    {
        //await reportGenerator.GeneratePdfAndShowAsync(id);
        //return ResultHelper.Ok("PDF invoice generated.");

        var pdfBytes = await reportGenerator.GeneratePdfBytesAsync(id);
        var pdfStream = new MemoryStream(pdfBytes);
        return TypedResults.Stream(pdfStream, contentType: "application/octet-stream", fileDownloadName: "invoice.pdf");
    }
}