namespace Gdn.Presentation.Shared.Models.Invoices;

public class InvoiceInputModel
{
    public int? Id { get; set; }
    public string Number { get; set; } = default!;
    public DateOnly Date { get; set; }
    public int CustomerId { get; set; }

    public IEnumerable<InvoiceRowInputModel>? Rows { get; set; }
}

public class InvoiceRowInputModel
{
    public long? Id { get; init; }
    public string? Description { get; init; }
    public decimal? Quantity { get; init; }
    public decimal? UnitPrice { get; init; }
    public int? MeasurementUnitId { get; init; }
    public int? TaxRateId { get; init; }
    public int? ProductId { get; init; }
}