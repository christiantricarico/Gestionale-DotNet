using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public partial class Product : TrackedEntity<int>
{
    public string Code { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Stock { get; set; }

    public int? ProductCategoryId { get; set; }
    public ProductCategory? ProductCategory { get; set; }

    public int? TaxRateId { get; set; }
    public TaxRate? TaxRate { get; set; }
}
