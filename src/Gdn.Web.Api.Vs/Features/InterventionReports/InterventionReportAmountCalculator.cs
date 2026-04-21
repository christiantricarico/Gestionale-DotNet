using Gdn.Domain.Models;

namespace Gdn.Web.Api.Vs.Features.InterventionReports;

public static class InterventionReportAmountCalculator
{
    public static decimal CalculateTotal(InterventionReport report)
    {
        decimal netAmount = CalculateNetAmount(report);
        decimal taxAmount = CalculateTaxAmount(report);

        return netAmount + taxAmount;
    }

    private static decimal CalculateNetAmount(InterventionReport report)
    {
        decimal netAmount = 0m;

        foreach (var row in report.Rows)
        {
            var rowNetAmount = Math.Round((row.UnitPrice * row.Quantity) ?? 0m, 2, MidpointRounding.AwayFromZero);
            netAmount += rowNetAmount;
        }

        return netAmount;
    }

    private static decimal CalculateTaxAmount(InterventionReport report)
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
