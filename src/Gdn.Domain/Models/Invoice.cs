using Gdn.Domain.Models.Bases;
using Gdn.Domain.Models.Enums;

namespace Gdn.Domain.Models;

public class Invoice : TrackedEntity<int>
{
    public string Number { get; set; } = default!;
    public DateOnly Date { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public ICollection<InvoiceRow> Rows { get; set; } = new List<InvoiceRow>();
}

public class InvoiceRow : TrackedEntity<long>
{
    public string RowType { get; set; } = DocumentRowType.DESCRIPTIVE;
    public string? Description { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }

    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public int? MeasurementUnitId { get; set; }
    public MeasurementUnit? MeasurementUnit { get; set; }

    public int? TaxRateId { get; set; }
    public TaxRate? TaxRate { get; set; }
}
