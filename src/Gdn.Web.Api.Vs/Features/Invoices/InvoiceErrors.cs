namespace Gdn.Web.Api.Vs.Features.Invoices;

public class InvoiceErrors
{
    public static Error InvalidInput(string propertyName) => new("Invoice:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound(int id) => new("Invoice:NotFound", $"Invoice with Id={id} not found");
    public static Error HasPayments(int id) => new("Invoice:HasPayments", $"Invoice with Id={id} has dues with recorded payments and cannot be deleted");
    public static Error CannotReduceInvoiceAmount() => new("Invoice:CannotReduceAmount", "The invoice total cannot be reduced because the outstanding balance on the remaining dues is already fully paid.");
}
