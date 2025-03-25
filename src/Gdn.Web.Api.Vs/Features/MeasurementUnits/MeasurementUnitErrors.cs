namespace Gdn.Web.Api.Vs.Features.MeasurementUnits;

public static class MeasurementUnitErrors
{
    public static Error InvalidInput(string propertyName) => new("MeasurementUnit:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound(int id) => new("MeasurementUnit:NotFound", $"MeasurementUnit with Id={id} not found");
}
