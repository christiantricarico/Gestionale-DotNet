namespace Gdn.Presentation.Shared.Models.TaxRates;

public class TaxRateViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Rate { get; set; }
    public int? TaxRateNatureId { get; set; }
    public string? TaxRateNatureName { get; set; }
}
