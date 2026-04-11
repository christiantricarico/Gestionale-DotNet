using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Microsoft.Extensions.Options;

namespace Gdn.Web.Api.Vs.Features.CreditNotes.Xml;

public class CreditNoteXmlFileNameGenerator(
    IOptions<AppSettings> appSettings,
    ICreditNoteRepository creditNoteRepository)
{
    public async Task<string> GenerateAsync(int creditNoteId)
    {
        CreditNote? creditNote = await creditNoteRepository.GetAsync(creditNoteId);
        if (creditNote is null)
            throw new InvalidOperationException($"Credit note with ID {creditNoteId} not found.");

        string fileName = $"IT{appSettings.Value.CompanyData.VatNumber}_NC{DateTime.Today.Year.ToString().Substring(2, 2)}{creditNote.Number.PadLeft(6, '0')}";
        string fileExtension = "xml";
        string fullFileName = $"{fileName}.{fileExtension}";
        return fullFileName;
    }
}
