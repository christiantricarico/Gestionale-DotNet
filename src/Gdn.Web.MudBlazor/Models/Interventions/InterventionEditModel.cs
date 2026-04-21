using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Interventions;

public class InterventionEditModel
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

    public bool IsInvoiced { get; set; }
    public int? InvoiceId { get; set; }

    public ICollection<InterventionRowEditModel> Rows { get; set; } = [];
}

public class InterventionRowEditModel
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
