using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Products;

public class ProductInputModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Codice richiesto.")]
    [StringLength(50)]
    public string Code { get; set; } = default!;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Tipo richiesto.")]
    public string Type { get; set; } = "PRD";

    public int? MeasurementUnitId { get; set; }
    public int? TaxRateId { get; set; }
    public int? ProductCategoryId { get; set; }
}
