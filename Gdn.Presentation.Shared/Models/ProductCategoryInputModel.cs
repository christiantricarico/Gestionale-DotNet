namespace Gdn.Presentation.Shared.Models;

public class ProductCategoryInputModel
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
}
