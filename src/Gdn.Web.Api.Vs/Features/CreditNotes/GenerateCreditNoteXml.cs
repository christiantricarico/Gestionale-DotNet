using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.CreditNotes.Xml;
using System.Net.Mime;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class GenerateCreditNoteXml
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/creditnotes/xml/{id:int}", HandlerAsync).WithTags(Tags.CreditNotes);
        }
    }

    private static async Task<IResult> HandlerAsync(
        int id,
        CreditNoteXmlGenerator xmlGenerator,
        CreditNoteXmlFileNameGenerator xmlFileNameGenerator)
    {
        var stream = await xmlGenerator.GenerateXmlStream(id);
        stream.Position = 0;

        string xmlFileName = await xmlFileNameGenerator.GenerateAsync(id);

        return TypedResults.File(stream,
            contentType: MediaTypeNames.Application.Octet,
            fileDownloadName: xmlFileName);
    }
}
