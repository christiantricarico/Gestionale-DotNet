using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Invoices;

public class InvoiceViewModel
{
    public int Id { get; set; }

    [Display(Name = "Numero")]
    public string Number { get; set; } = default!;

    [Display(Name = "Data")]
    public DateOnly Date { get; set; }

    public int? CustomerId { get; set; }

    [Display(Name = "Cliente")]
    public string? CustomerName { get; set; }

    public decimal? StampDutyAmount { get; set; }
    public bool StampDutyChargedToCustomer { get; set; }

    [Display(Name = "Stato pagamento")]
    public string PaymentStatus { get; set; } = "Non pagata";

    public IEnumerable<InvoiceRowViewModel> Rows { get; set; } = [];
    public IEnumerable<InvoiceDueViewModel> Dues { get; set; } = [];
}

public class InvoiceRowViewModel
{
    public long Id { get; set; }

    [Display(Name = "Tipo riga")]
    public string RowType { get; set; } = default!;

    [Display(Name = "Descrizione")]
    public string? Description { get; set; }

    [Display(Name = "Quantità")]
    public decimal? Quantity { get; set; }

    [Display(Name = "Prezzo unitario")]
    public decimal? UnitPrice { get; set; }

    public int? MeasurementUnitId { get; set; }

    [Display(Name = "Unità di misura")]
    public string? MeasurementUnitName { get; set; }

    public int? TaxRateId { get; set; }

    [Display(Name = "Aliquota IVA")]
    public string? TaxRateName { get; set; }

    [Display(Name = "Totale")]
    public decimal TotalAmount => (Quantity ?? 0) * (UnitPrice ?? 0);
}

public class InvoiceDueViewModel
{
    public int Id { get; set; }

    [Display(Name = "Scadenza")]
    public DateOnly Date { get; set; }

    [Display(Name = "Importo")]
    public decimal Amount { get; set; }

    [Display(Name = "Pagato")]
    public decimal PaidAmount { get; set; }

    public bool IsPaid { get; set; }

    [Display(Name = "Stato")]
    public string Status => IsPaid ? "Pagata" : PaidAmount != 0m ? "Parzialmente pagata" : "Non pagata";
}
