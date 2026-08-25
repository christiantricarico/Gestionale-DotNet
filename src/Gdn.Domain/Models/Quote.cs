using Gdn.Domain.Models.Bases;
using Gdn.Domain.Models.Enums;

namespace Gdn.Domain.Models;

public class Quote : TrackedEntity<int>
{
    public string Number { get; set; } = default!;
    public DateOnly Date { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public bool IsAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }

    public ICollection<QuoteRow> Rows { get; set; } = [];
}

public class QuoteRow : TrackedEntity<long>
{
    public string RowType { get; set; } = DocumentRowType.DESCRIPTIVE;
    public string? Description { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }

    public int QuoteId { get; set; }
    public Quote Quote { get; set; } = default!;

    public int? MeasurementUnitId { get; set; }
    public MeasurementUnit? MeasurementUnit { get; set; }

    public int? TaxRateId { get; set; }
    public TaxRate? TaxRate { get; set; }

    public int? ProductId { get; set; }
    public Product? Product { get; set; }
}
