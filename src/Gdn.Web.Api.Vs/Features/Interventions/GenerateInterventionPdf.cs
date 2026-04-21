using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Interventions.Reports;

namespace Gdn.Web.Api.Vs.Features.Interventions;

public class GenerateInterventionPdf
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/interventions/pdf/{id:int}", HandlerAsync).WithTags(Tags.Interventions);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, InterventionReportGenerator reportGenerator)
    {
        var pdfBytes = await reportGenerator.GeneratePdfBytesAsync(id);
        var pdfStream = new MemoryStream(pdfBytes);
        return TypedResults.Stream(pdfStream, contentType: "application/octet-stream", fileDownloadName: "intervention_report.pdf");
    }
}
