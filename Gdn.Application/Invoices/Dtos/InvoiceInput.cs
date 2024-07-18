using Gdn.Domain.Models;

namespace Gdn.Application.Invoices.Dtos;

public sealed class InvoiceInput
{
    public int? Id { get; init; }
    public required string Number { get; init; }
    public DateOnly Date { get; init; }
    public int CustomerId { get; init; }

    public IEnumerable<InvoiceRow>? Rows { get; init; }
}

public sealed class InvoiceRowInput
{
    public long? Id { get; init; }
    public string? Description { get; init; }
    public decimal? Quantity { get; init; }
    public decimal? UnitPrice { get; init; }
    public int? MeasurementUnitId { get; init; }
    public int? TaxRateId { get; init; }
    public int? ProductId { get; init; }
}