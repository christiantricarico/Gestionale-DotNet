namespace Gdn.Presentation.Shared.Models.ProductCategories;

public class ProductCategoryViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Level { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
}
