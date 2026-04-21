using Gdn.Domain.Models;

namespace Gdn.Web.Api.Vs.Features.Interventions;

public static class InterventionAmountCalculator
{
    public static decimal CalculateTotal(Intervention report)
    {
        decimal netAmount = CalculateNetAmount(report);
        decimal taxAmount = CalculateTaxAmount(report);

        return netAmount + taxAmount;
    }

    private static decimal CalculateNetAmount(Intervention report)
    {
        decimal netAmount = 0m;

        foreach (var row in report.Rows)
        {
            var rowNetAmount = Math.Round((row.UnitPrice * row.Quantity) ?? 0m, 2, MidpointRounding.AwayFromZero);
            netAmount += rowNetAmount;
        }

        return netAmount;
    }

    public static decimal CalculateExemptVatTotal(Intervention intervention)
    {
        decimal exemptTotal = 0m;

        foreach (var row in intervention.Rows)
        {
            if (row.TaxRate?.Rate == 0m)
            {
                var rowAmount = Math.Round((row.UnitPrice * row.Quantity) ?? 0m, 2, MidpointRounding.AwayFromZero);
                exemptTotal += rowAmount;
            }
        }

        return exemptTotal;
    }

    private static decimal CalculateTaxAmount(Intervention report)
    {
        decimal taxAmount = 0m;

        var rowsGroupedByTaxRate = report.Rows
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
