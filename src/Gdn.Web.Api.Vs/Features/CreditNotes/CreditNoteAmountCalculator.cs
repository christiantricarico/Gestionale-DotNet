using Gdn.Domain.Models;

namespace Gdn.Web.Api.Vs.Features.CreditNotes;

/// <summary>
/// Calculates the total payable amount for a credit note, reused when auto-generating dues
/// and when rebuilding XML/PDF data.
/// Requires the credit note to have <see cref="CreditNote.Rows"/> loaded with <see cref="CreditNoteRow.TaxRate"/>.
/// </summary>
internal static class CreditNoteAmountCalculator
{
    public static decimal CalculateTotal(CreditNote creditNote)
    {
        decimal total = 0m;

        var rowsByTaxRate = creditNote.Rows.GroupBy(r => r.TaxRate?.Rate ?? 0m);

        foreach (var group in rowsByTaxRate)
        {
            decimal net = group.Sum(r =>
                Math.Round((r.UnitPrice ?? 0m) * (r.Quantity ?? 0m), 2, MidpointRounding.AwayFromZero));

            decimal tax = Math.Round(net * group.Key / 100m, 2, MidpointRounding.AwayFromZero);

            total += net + tax;
        }

        if (creditNote.StampDutyAmount.HasValue && creditNote.StampDutyChargedToCustomer)
            total += creditNote.StampDutyAmount.Value;

        return total;
    }
}
