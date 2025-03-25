using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.Fluentblazor.Models.MeasurementUnits;

public class MeasurementUnitInputModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Codice richiesto.")]
    [StringLength(10)]
    public string Code { get; set; } = default!;

    [StringLength(255)]
    public string? Name { get; set; }

    public string? Description { get; set; }
}
