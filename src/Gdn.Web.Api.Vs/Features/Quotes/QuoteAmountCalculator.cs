using Gdn.Domain.Models;

namespace Gdn.Web.Api.Vs.Features.Quotes;

public static class QuoteAmountCalculator
{
    public static decimal CalculateTotal(Quote quote)
    {
        decimal netAmount = CalculateNetAmount(quote);
        decimal taxAmount = CalculateTaxAmount(quote);

        return netAmount + taxAmount;
    }

    private static decimal CalculateNetAmount(Quote quote)
    {
        decimal netAmount = 0m;

        foreach (var row in quote.Rows)
        {
            var rowNetAmount = Math.Round((row.UnitPrice * row.Quantity) ?? 0m, 2, MidpointRounding.AwayFromZero);
            netAmount += rowNetAmount;
        }

        return netAmount;
    }

    public static decimal CalculateExemptVatTotal(Quote quote)
    {
        decimal exemptTotal = 0m;

        foreach (var row in quote.Rows)
        {
            if (row.TaxRate?.Rate == 0m)
            {
                var rowAmount = Math.Round((row.UnitPrice * row.Quantity) ?? 0m, 2, MidpointRounding.AwayFromZero);
                exemptTotal += rowAmount;
            }
        }

        return exemptTotal;
    }

    private static decimal CalculateTaxAmount(Quote quote)
    {
        decimal taxAmount = 0m;

        var rowsGroupedByTaxRate = quote.Rows
            .Where(r => r.TaxRate is not null)
            .GroupBy(x => x.TaxRate);

        foreach (var taxGroup in rowsGroupedByTaxRate)
        {
            decimal groupNetAmount = taxGroup.Sum(x => Math.Round((x.UnitPrice * x.Quantity) ?? 0, 2, MidpointRounding.AwayFromZero));

            decimal taxRate = taxGroup.Key?.Rate ?? 0;
            decimal groupTaxAmount = groupNetAmount * taxRate / 100;
            taxAmount += Math.Round(groupTaxAmount, 2, MidpointRounding.AwayFromZero);
        }

        return taxAmount;
    }
}
