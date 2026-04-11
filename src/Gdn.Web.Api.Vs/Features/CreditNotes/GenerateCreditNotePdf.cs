using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class GenerateCreditNotePdf
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/creditnotes/pdf/{id:int}", Handler).WithTags(Tags.CreditNotes);
        }
    }

    private static async Task<IResult> Handler(int id, Reports.CreditNoteReportGenerator reportGenerator)
    {
        var pdfBytes = await reportGenerator.GeneratePdfBytesAsync(id);
        var pdfStream = new MemoryStream(pdfBytes);
        return TypedResults.Stream(pdfStream, contentType: "application/octet-stream", fileDownloadName: "credit_note.pdf");
    }
}
