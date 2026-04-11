namespace Gdn.Web.Api.Vs.Features.CreditNotes;

public class CreditNoteErrors
{
    public static Error NotFound(int id) => new("CreditNote:NotFound", $"Credit note with Id={id} not found");
    public static Error HasPayments(int id) => new("CreditNote:HasPayments", $"Credit note with Id={id} has dues with recorded payments and cannot be deleted");
    public static Error CannotReduceCreditNoteAmount() => new("CreditNote:CannotReduceAmount", "The credit note total cannot be reduced because the outstanding balance on the remaining dues is already fully paid.");
}
