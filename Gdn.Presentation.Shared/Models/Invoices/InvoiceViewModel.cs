namespace Gdn.Presentation.Shared.Models.Invoices;

public class InvoiceViewModel
{
    public int Id { get; set; }
    public string Number { get; set; } = default!;
    public DateOnly Date { get; set; }
    public int CustomerId { get; set; }

    public IEnumerable<InvoiceRowViewModel>? Rows { get; set; }
}

public class InvoiceRowViewModel
{
    public long Id { get; set; }
    public string Description { get; set; } = default!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int MeasurementUnitId { get; set; }
    public int TaxRateId { get; set; }
    public int ProductId { get; set; }
}