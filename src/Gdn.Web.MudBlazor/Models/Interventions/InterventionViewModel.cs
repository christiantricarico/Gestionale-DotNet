using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Interventions;

public class InterventionViewModel
{
    public int Id { get; set; }

    [Display(Name = "Numero")]
    public string Number { get; set; } = default!;

    [Display(Name = "Data")]
    public DateOnly Date { get; set; }

    public int? CustomerId { get; set; }

    [Display(Name = "Cliente")]
    public string? CustomerName { get; set; }

    public bool IsInvoiced { get; set; }
    public int? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }

    public IEnumerable<InterventionRowViewModel> Rows { get; set; } = [];
}

public class InterventionRowViewModel
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
