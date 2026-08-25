using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Quotes.Reports;

namespace Gdn.Web.Api.Vs.Features.Quotes;

public class GenerateQuotePdf
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/quotes/pdf/{id:int}", HandlerAsync).WithTags(Tags.Quotes);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, QuoteReportGenerator reportGenerator)
    {
        var pdfBytes = await reportGenerator.GeneratePdfBytesAsync(id);
        if (pdfBytes is null)
            return ResultHelper.NotFound(QuoteErrors.NotFound(id));

        var pdfStream = new MemoryStream(pdfBytes);
        return TypedResults.Stream(pdfStream, contentType: "application/octet-stream", fileDownloadName: "quote.pdf");
    }
}
