﻿using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.Fluentblazor.Models.TaxRates;

public class TaxRateInputModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Codice richiesto.")]
    [StringLength(10)]
    public string Code { get; set; } = default!;

    [StringLength(255)]
    public string? Name { get; set; }

    public string? Description { get; set; }
    public decimal Rate { get; set; }
    public int? TaxRateNatureId { get; set; }
}
