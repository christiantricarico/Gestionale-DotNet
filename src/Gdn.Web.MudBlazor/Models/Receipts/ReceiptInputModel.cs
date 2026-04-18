using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Receipts;

public class ReceiptInputModel
{
    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public DateTime? DateWithTime
    {
        get => Date.ToDateTime(TimeOnly.MinValue);
        set
        {
            if (value.HasValue)
                Date = DateOnly.FromDateTime(value.Value);
        }
    }

    [Display(Name = "Importo")]
    public decimal Amount { get; set; }

    [Display(Name = "Metodo di pagamento")]
    public int? PaymentMethodId { get; set; }
}

public class CustomerDueViewModel
{
    public int DueId { get; set; }
    public DateOnly DueDate { get; set; }
    public string? DocumentNumber { get; set; }
    public string DocumentType { get; set; } = default!;
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }

    public decimal AllocatedAmount { get; set; }
    public bool IsSelected { get; set; }
}
