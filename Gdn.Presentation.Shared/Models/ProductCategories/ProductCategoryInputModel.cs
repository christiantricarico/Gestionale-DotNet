using System.ComponentModel.DataAnnotations;

namespace Gdn.Presentation.Shared.Models.ProductCategories;

public class ProductCategoryInputModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(10)]
    public string Code { get; set; } = default!;

    [StringLength(255)]
    public string? Name { get; set; }

    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
}
