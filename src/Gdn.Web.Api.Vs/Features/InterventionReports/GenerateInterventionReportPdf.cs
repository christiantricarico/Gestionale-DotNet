using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.InterventionReports.Reports;

namespace Gdn.Web.Api.Vs.Features.InterventionReports;

public class GenerateInterventionReportPdf
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/interventionreports/pdf/{id:int}", HandlerAsync).WithTags(Tags.InterventionReports);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, InterventionReportReportGenerator reportGenerator)
    {
        var pdfBytes = await reportGenerator.GeneratePdfBytesAsync(id);
        var pdfStream = new MemoryStream(pdfBytes);
        return TypedResults.Stream(pdfStream, contentType: "application/octet-stream", fileDownloadName: "intervention_report.pdf");
    }
}
