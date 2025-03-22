using System.ComponentModel.DataAnnotations;

namespace Gdn.Presentation.Shared.Models.Invoices;

public class InvoiceInputModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Numero è richiesto.")]
    public string Number { get; set; } = default!;

    [Required(ErrorMessage = "Data è richiesto.")]
    public DateTime? DateWithTime { get; set; }
    public DateOnly Date => DateOnly.FromDateTime(DateWithTime ?? DateTime.MinValue);

    [Required(ErrorMessage = "Cliente è richiesto.")]
    public int? CustomerId { get; set; }

    public ICollection<InvoiceRowInputModel> Rows { get; set; } = [];
}

public class InvoiceRowInputModel
{
    public int InputStatus { get; set; }
    public long? Id { get; set; }

    [Required(ErrorMessage = "Descrizione è richiesto.")]
    public string? Description { get; set; }

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int? MeasurementUnitId { get; set; }
    public int? TaxRateId { get; set; }
}