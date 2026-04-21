namespace Gdn.Web.Api.Vs.Features.Interventions;

public static class InterventionErrors
{
    public static Error NotFound(int id) => new("Intervention:NotFound", $"Intervention report with Id={id} not found");
    public static Error AlreadyInvoiced(int id) => new("Intervention:AlreadyInvoiced", $"Intervention report with Id={id} is already invoiced");
    public static Error InvalidCustomer(int id) => new("Intervention:InvalidCustomer", $"Intervention report with Id={id} belongs to a different customer");
    public static Error EmptyRows(int id) => new("Intervention:EmptyRows", $"Intervention report with Id={id} has no rows to invoice");
    public static Error InvalidMeasurementUnit(int id) => new("Intervention:InvalidMeasurementUnit", $"Measurement unit with Id={id} not found");
    public static Error InvalidTaxRate(int id) => new("Intervention:InvalidTaxRate", $"Tax rate with Id={id} not found");
}
