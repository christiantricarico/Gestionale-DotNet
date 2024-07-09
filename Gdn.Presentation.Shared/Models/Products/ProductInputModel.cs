using System.ComponentModel.DataAnnotations;

namespace Gdn.Presentation.Shared.Models.Products;

public class ProductInputModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(10)]
    public string Code { get; set; } = default!;

    [StringLength(255)]
    public string? Name { get; set; }

    public string? Description { get; set; }
    public int? TaxRateId { get; set; }
    public int? ProductCategoryId { get; set; }
}
