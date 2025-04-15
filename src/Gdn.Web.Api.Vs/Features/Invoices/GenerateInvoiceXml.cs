using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Invoices.Xml;
using System.Net.Mime;

namespace Gdn.Web.Api.Vs.Features.Invoices;

public class GenerateInvoiceXml
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/invoices/xml/{id}", Handler).WithTags(Tags.Invoices);
        }
    }

    private static async Task<IResult> Handler(int id, InvoiceXmlGenerator xmlGenerator)
    {
        var stream = await xmlGenerator.GenerateXmlStream(id);
        stream.Position = 0; // Reset the stream position to the beginning

        return TypedResults.File(stream,
            contentType: MediaTypeNames.Application.Octet,
            fileDownloadName: "invoice.xml");
    }
}
