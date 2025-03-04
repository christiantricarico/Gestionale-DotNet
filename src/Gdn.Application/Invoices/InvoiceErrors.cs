namespace Gdn.Application.Invoices;

public static class InvoiceErrors
{
    public static Error InvalidInput(string propertyName) => new("Invoice:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound(int id) => new("Invoice:NotFound", $"Invoice with Id={id} not found");
}
