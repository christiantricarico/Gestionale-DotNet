using Gdn.Domain.Models;

namespace Gdn.Web.Api.Vs.Features.Invoices;

/// <summary>
/// Calculates the total payable amount for an invoice, reused when auto-generating dues
/// and when rebuilding XML/PDF data.
/// Requires the invoice to have <see cref="Invoice.Rows"/> loaded with <see cref="InvoiceRow.TaxRate"/>.
/// </summary>
internal static class InvoiceAmountCalculator
{
    public static decimal CalculateTotal(Invoice invoice)
    {
        decimal total = 0m;

        var rowsByTaxRate = invoice.Rows.GroupBy(r => r.TaxRate?.Rate ?? 0m);

        foreach (var group in rowsByTaxRate)
        {
            decimal net = group.Sum(r =>
                Math.Round((r.UnitPrice ?? 0m) * (r.Quantity ?? 0m), 2, MidpointRounding.AwayFromZero));

            decimal tax = Math.Round(net * group.Key / 100m, 2, MidpointRounding.AwayFromZero);

            total += net + tax;
        }

        if (invoice.StampDutyAmount.HasValue && invoice.StampDutyChargedToCustomer)
            total += invoice.StampDutyAmount.Value;

        return total;
    }
}
