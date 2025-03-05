using System.ComponentModel.DataAnnotations;

namespace Gdn.Presentation.Shared.Models.MeasurementUnits;

public class MeasurementUnitViewModel
{
    public int Id { get; set; }

    [Display(Name = "Codice")]
    public string Code { get; set; } = default!;

    [Display(Name = "Nome")]
    public string? Name { get; set; }

    [Display(Name = "Descrizione")]
    public string? Description { get; set; }
}
