using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Invoices;

public class InvoiceEditModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Numero è richiesto.")]
    [Display(Name = "Numero")]
    public int? Number { get; set; }

    [Display(Name = "Data")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Data è richiesto.")]
    public DateTime? DateWithTime { get; set; }

    public int? CustomerId { get; set; }

    [Display(Name = "Cliente")]
    public string? CustomerName { get; set; }

    public decimal? StampDutyAmount { get; set; }
    public bool StampDutyChargedToCustomer { get; set; }
    public ICollection<int> InterventionReportIds { get; set; } = [];

    public ICollection<InvoiceRowEditModel> Rows { get; set; } = [];
    public ICollection<InvoiceDueEditModel> Dues { get; set; } = [];
}

public class InterventionReportOptionViewModel
{
    public int Id { get; set; }
    public string Number { get; set; } = default!;
    public DateOnly Date { get; set; }
    public decimal TotalAmount { get; set; }
}

public class InvoiceRowEditModel
{
    public int InputStatus { get; set; }
    public long? Id { get; set; }

    [Display(Name = "Tipo riga")]
    public string RowType { get; set; } = default!;

    [Display(Name = "Descrizione")]
    public string? Description { get; set; }

    [Display(Name = "Quantità")]
    public decimal Quantity { get; set; }

    [Display(Name = "Prezzo unitario")]
    public decimal UnitPrice { get; set; }

    public int? MeasurementUnitId { get; set; }

    [Display(Name = "Unità di misura")]
    public string? MeasurementUnitName { get; set; }

    public int? TaxRateId { get; set; }

    [Display(Name = "Aliquota IVA")]
    public string? TaxRateName { get; set; }

    public decimal? TaxRateValue { get; set; }

    [Display(Name = "Totale")]
    public decimal TotalAmount => Quantity * UnitPrice;
}

public class InvoiceDueEditModel
{
    public int InputStatus { get; set; }
    public int? Id { get; set; }

    [Required(ErrorMessage = "Data scadenza richiesta.")]
    [Display(Name = "Data scadenza")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Importo richiesto.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Importo deve essere maggiore di zero.")]
    [Display(Name = "Importo")]
    public decimal Amount { get; set; }

    [Display(Name = "Pagato")]
    public decimal PaidAmount { get; set; }

    public bool IsPaid { get; set; }

    [Display(Name = "Stato")]
    public string Status => IsPaid ? "Pagata" : PaidAmount != 0m ? "Parzialmente pagata" : "Non pagata";

    public bool HasPayments => PaidAmount != 0m;
}
