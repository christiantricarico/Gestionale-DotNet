using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public partial class Product : TrackedSoftDeletableEntity<int>
{
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
    public string? BillingDescription { get; set; }
    public decimal Stock { get; set; }

    public int? ProductCategoryId { get; set; }
    public ProductCategory? ProductCategory { get; set; }
}
