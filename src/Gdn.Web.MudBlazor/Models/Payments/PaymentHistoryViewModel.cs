using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Payments;

public class PaymentHistoryViewModel
{
    public int Id { get; set; }

    [Display(Name = "Data")]
    public DateOnly Date { get; set; }

    [Display(Name = "Importo")]
    public decimal Amount { get; set; }

    [Display(Name = "Metodo")]
    public string? PaymentMethodName { get; set; }

    public IEnumerable<PaymentAllocationHistoryViewModel> Allocations { get; set; } = [];
}

public class PaymentAllocationHistoryViewModel
{
    public int DueId { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal AllocationAmount { get; set; }
}
