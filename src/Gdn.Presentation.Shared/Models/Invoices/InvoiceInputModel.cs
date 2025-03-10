namespace Gdn.Presentation.Shared.Models.Invoices;

public class InvoiceInputModel
{
    public int? Id { get; set; }
    public string Number { get; set; } = default!;
    public DateOnly Date { get; set; }
    public int CustomerId { get; set; }
}

public class InvoiceRowInputModel
{
    public long? Id { get; set; }
    public string? Description { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public int? MeasurementUnitId { get; set; }
    public int? TaxRateId { get; set; }
}