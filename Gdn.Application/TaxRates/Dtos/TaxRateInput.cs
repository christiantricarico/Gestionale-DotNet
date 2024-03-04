namespace Gdn.Application.TaxRates.Dtos;

public sealed class TaxRateInput
{
    public int? Id { get; set; }
    public string Code { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? TaxRateNatureId { get; set; }
}
