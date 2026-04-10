using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Receipts;

public class ReceiptHistoryViewModel
{
    public int PaymentId { get; set; }

    [Display(Name = "Data")]
    public DateOnly Date { get; set; }

    [Display(Name = "Importo")]
    public decimal Amount { get; set; }

    [Display(Name = "Metodo")]
    public string? PaymentMethodName { get; set; }

    public IEnumerable<ReceiptCoveredDueViewModel> CoveredDues { get; set; } = [];
}

public class ReceiptCoveredDueViewModel
{
    public int DueId { get; set; }
    public DateOnly DueDate { get; set; }
    public string? DocumentNumber { get; set; }
    public string DocumentType { get; set; } = default!;
    public decimal AllocationAmount { get; set; }
}
