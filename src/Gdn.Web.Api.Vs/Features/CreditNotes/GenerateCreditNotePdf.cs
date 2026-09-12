using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class GenerateCreditNotePdf
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/creditnotes/pdf/{id:int}", HandlerAsync).WithTags(Tags.CreditNotes).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, Reports.CreditNoteReportGenerator reportGenerator)
    {
        var pdfBytes = await reportGenerator.GeneratePdfBytesAsync(id);
        var pdfStream = new MemoryStream(pdfBytes);
        return TypedResults.Stream(pdfStream, contentType: "application/octet-stream", fileDownloadName: "credit_note.pdf");
    }
}
