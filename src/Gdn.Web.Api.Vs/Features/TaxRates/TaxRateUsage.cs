using Gdn.Domain.Data.Repositories;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

internal static class TaxRateUsage
{
    /// <summary>
    /// Counts document rows (Fattura, Nota di credito, Preventivo, Rapporto di intervento) that
    /// reference the given tax rate. Unlike the product/customer/system defaults, this usage blocks
    /// deletion outright: nulling the tax rate on an already-documented row would silently change a
    /// figure that was already invoiced.
    /// </summary>
    public static async Task<int> CountDocumentRowsAsync(
        int taxRateId,
        IInvoiceRowRepository invoiceRowRepository,
        ICreditNoteRowRepository creditNoteRowRepository,
        IQuoteRowRepository quoteRowRepository,
        IInterventionRowRepository interventionRowRepository)
    {
        var invoiceRowCount = await invoiceRowRepository.CountAsync(r => r.TaxRateId == taxRateId);
        var creditNoteRowCount = await creditNoteRowRepository.CountAsync(r => r.TaxRateId == taxRateId);
        var quoteRowCount = await quoteRowRepository.CountAsync(r => r.TaxRateId == taxRateId);
        var interventionRowCount = await interventionRowRepository.CountAsync(r => r.TaxRateId == taxRateId);

        return invoiceRowCount + creditNoteRowCount + quoteRowCount + interventionRowCount;
    }
}
