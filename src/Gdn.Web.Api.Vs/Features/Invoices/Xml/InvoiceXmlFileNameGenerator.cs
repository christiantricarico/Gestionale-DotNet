using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Microsoft.Extensions.Options;

namespace Gdn.Web.Api.Vs.Features.Invoices.Xml;

public class InvoiceXmlFileNameGenerator(
    IOptions<CompanyData> companyData,
    IInvoiceRepository invoiceRepository)
{
    public async Task<string> GenerateAsync(int invoiceId)
    {
        Invoice? invoice = await invoiceRepository.GetAsync(invoiceId);
        if (invoice is null)
            throw new InvalidOperationException($"Invoice with ID {invoiceId} not found.");

        string fileName = $"IT{companyData.Value.VatNumber}_FA{DateTime.Today.Year.ToString().Substring(2, 2)}{invoice.Number.PadLeft(6, '0')}";
        string fileExtension = "xml";
        string fullFileName = $"{fileName}.{fileExtension}";
        return fullFileName;
    }
}