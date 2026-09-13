using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models.Settings;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Settings;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public class GetTaxRateUsage
{
    public record GetTaxRateUsageResponse(int ProductCount, int CustomerCount, bool IsDocumentDefault, int DocumentRowCount);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/taxrates/{id:int}/usage", HandlerAsync).WithTags(Tags.TaxRates);
        }
    }

    private static async Task<IResult> HandlerAsync(
        int id,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        ISettingsService settingsService,
        IInvoiceRowRepository invoiceRowRepository,
        ICreditNoteRowRepository creditNoteRowRepository,
        IQuoteRowRepository quoteRowRepository,
        IInterventionRowRepository interventionRowRepository)
    {
        var productCount = await productRepository.CountAsync(p => p.TaxRateId == id);
        var customerCount = await customerRepository.CountAsync(c => c.DefaultTaxRateId == id);
        var documentSettings = await settingsService.GetAsync<DocumentSettings>();
        var documentRowCount = await TaxRateUsage.CountDocumentRowsAsync(id, invoiceRowRepository, creditNoteRowRepository, quoteRowRepository, interventionRowRepository);

        return ResultHelper.Ok(new GetTaxRateUsageResponse(productCount, customerCount, documentSettings.DefaultTaxRateId == id, documentRowCount));
    }
}
