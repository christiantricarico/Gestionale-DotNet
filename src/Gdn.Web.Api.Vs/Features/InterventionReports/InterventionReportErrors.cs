namespace Gdn.Web.Api.Vs.Features.InterventionReports;

public static class InterventionReportErrors
{
    public static Error NotFound(int id) => new("InterventionReport:NotFound", $"Intervention report with Id={id} not found");
    public static Error AlreadyInvoiced(int id) => new("InterventionReport:AlreadyInvoiced", $"Intervention report with Id={id} is already invoiced");
    public static Error InvalidCustomer(int id) => new("InterventionReport:InvalidCustomer", $"Intervention report with Id={id} belongs to a different customer");
    public static Error EmptyRows(int id) => new("InterventionReport:EmptyRows", $"Intervention report with Id={id} has no rows to invoice");
    public static Error InvalidMeasurementUnit(int id) => new("InterventionReport:InvalidMeasurementUnit", $"Measurement unit with Id={id} not found");
    public static Error InvalidTaxRate(int id) => new("InterventionReport:InvalidTaxRate", $"Tax rate with Id={id} not found");
}
