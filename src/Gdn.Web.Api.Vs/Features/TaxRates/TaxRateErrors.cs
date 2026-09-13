namespace Gdn.Web.Api.Vs.Features.TaxRates;

public static class TaxRateErrors
{
    public static Error InvalidInput(string propertyName) => new("TaxRate:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound(int id) => new("TaxRate:NotFound", $"Tax rate with Id={id} not found");
    public static Error InUseOnDocumentRows(int id) => new("TaxRate:InUseOnDocumentRows",
        $"Tax rate with Id={id} is used on one or more document rows and cannot be deleted.");
}
