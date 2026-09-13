using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models.Settings;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Settings;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public class DeleteTaxRate
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/taxrates/{id:int}", HandlerAsync).WithTags(Tags.TaxRates);
        }
    }

    private static async Task<IResult> HandlerAsync(
        int id,
        IUnitOfWork unitOfWork,
        ISettingsService settingsService,
        IInvoiceRowRepository invoiceRowRepository,
        ICreditNoteRowRepository creditNoteRowRepository,
        IQuoteRowRepository quoteRowRepository,
        IInterventionRowRepository interventionRowRepository)
    {
        // A tax rate already used on a document row cannot be deleted: nulling it out would
        // silently change the VAT rate of an amount that has already been invoiced/documented.
        var documentRowCount = await TaxRateUsage.CountDocumentRowsAsync(id, invoiceRowRepository, creditNoteRowRepository, quoteRowRepository, interventionRowRepository);
        if (documentRowCount > 0)
            return ResultHelper.Conflict(TaxRateErrors.InUseOnDocumentRows(id));

        // Product.TaxRateId and Customer.DefaultTaxRateId are real foreign keys with ON DELETE SET
        // NULL, so the database clears those references on its own. The document default lives in
        // the generic JSON-backed Setting store instead, so it has to be cleared explicitly here.
        var documentSettings = await settingsService.GetAsync<DocumentSettings>();
        if (documentSettings.DefaultTaxRateId == id)
        {
            documentSettings.DefaultTaxRateId = null;
            await settingsService.SaveAsync(documentSettings);
        }

        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();
        await taxRateRepository.RemoveAsync(id);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
