using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public partial class ProductCategory : TrackedEntity<int>
{
    public string Code { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Level { get; set; }

    public int? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }
}
