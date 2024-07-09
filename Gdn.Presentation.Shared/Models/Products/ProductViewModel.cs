namespace Gdn.Presentation.Shared.Models.Products;

public class ProductViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? TaxRateId { get; set; }
    public string? TaxRateName { get; set; }
}
