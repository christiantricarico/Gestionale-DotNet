using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Payments;

public class PaymentInputModel
{
    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public DateTime? DateWithTime
    {
        get => Date.ToDateTime(TimeOnly.MinValue);
        set { if (value.HasValue) Date = DateOnly.FromDateTime(value.Value); }
    }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Importo deve essere maggiore di zero.")]
    [Display(Name = "Importo")]
    public decimal Amount { get; set; }

    [Display(Name = "Metodo di pagamento")]
    public int? PaymentMethodId { get; set; }

    public IList<PaymentAllocationModel> Allocations { get; set; } = [];
}

public class PaymentAllocationModel
{
    public int DueId { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal DueAmount { get; set; }
    public decimal DuePaidAmount { get; set; }
    public decimal DueRemainingAmount => DueAmount - DuePaidAmount;

    [Range(0, double.MaxValue)]
    [Display(Name = "Importo allocato")]
    public decimal Amount { get; set; }
}
