namespace Gdn.Web.Api.Vs.Features.Quotes;

public static class QuoteErrors
{
    public static Error NotFound(int id) => new("Quote:NotFound", $"Quote with Id={id} not found");
    public static Error AlreadyAccepted(int id) => new("Quote:AlreadyAccepted", $"Quote with Id={id} is already accepted");
    public static Error NotAccepted(int id) => new("Quote:NotAccepted", $"Quote with Id={id} is not accepted");
    public static Error InvalidMeasurementUnit(int id) => new("Quote:InvalidMeasurementUnit", $"Measurement unit with Id={id} not found");
    public static Error InvalidTaxRate(int id) => new("Quote:InvalidTaxRate", $"Tax rate with Id={id} not found");
}
