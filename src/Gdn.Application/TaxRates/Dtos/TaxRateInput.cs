namespace Gdn.Application.TaxRates.Dtos;

public sealed class TaxRateInput
{
    public int? Id { get; init; }
    public required string Code { get; set; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public decimal Rate { get; init; }
    public int? TaxRateNatureId { get; init; }
}
