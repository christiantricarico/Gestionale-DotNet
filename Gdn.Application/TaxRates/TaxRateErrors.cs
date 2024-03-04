namespace Gdn.Application.TaxRates;

public static class TaxRateErrors
{
    public static Error InvalidInput(string propertyName) => new("TaxRate:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound(int id) => new("TaxRate:NotFound", $"Tax rate with Id={id} not found");
}
