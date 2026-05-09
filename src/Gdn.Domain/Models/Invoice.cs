using Gdn.Domain.Models.Bases;
using Gdn.Domain.Models.Enums;

namespace Gdn.Domain.Models;

public class Invoice : TrackedEntity<int>
{
    public string Number { get; set; } = default!;
    public DateOnly Date { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public ICollection<InvoiceRow> Rows { get; set; } = [];

    public decimal? StampDutyAmount { get; set; }
    public bool StampDutyChargedToCustomer { get; set; }

    /// <summary>The dues (payment deadlines) associated with this invoice.</summary>
    public ICollection<Due> Dues { get; set; } = [];

    /// <summary>The intervention reports that have been invoiced and linked to this invoice.</summary>
    public ICollection<Intervention> Interventions { get; set; } = [];

    /// <summary>
    /// Returns <see langword="true"/> when all dues are fully paid.
    /// Returns <see langword="false"/> if there are no dues.
    /// </summary>
    public bool IsPaid => Dues.Count > 0 && Dues.All(d => d.IsPaid);
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

    public int? ProductId { get; set; }
    public Product? Product { get; set; }
}
