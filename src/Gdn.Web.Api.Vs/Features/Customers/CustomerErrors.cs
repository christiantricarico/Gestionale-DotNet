namespace Gdn.Web.Api.Vs.Features.Customers;

public static class CustomerErrors
{
    public static Error InvalidInput(string propertyName) => new("Customer:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound(int id) => new("Customer:NotFound", $"Customer with Id={id} not found");
}
