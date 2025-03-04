using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public class ProductCategory : RegistryEntity<int>
{
    public int Level { get; set; }

    public int? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }
}
