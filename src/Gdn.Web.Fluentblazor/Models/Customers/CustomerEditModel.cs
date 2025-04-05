using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.Fluentblazor.Models.Customers;

public class CustomerEditModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Codice richiesto.")]
    [StringLength(10)]
    public string Code { get; set; } = default!;

    [StringLength(255)]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [StringLength(20)]
    public string? FiscalCode { get; set; }

    [StringLength(20)]
    public string? VatNumber { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(255)]
    public string? Website { get; set; }

    [StringLength(255)]
    public string? Pec { get; set; }

    [StringLength(10)]
    public string? Sdi { get; set; }

    public string? Notes { get; set; }

    [StringLength(255)]
    public string? Street { get; set; }

    [StringLength(10)]
    public string? PostalCode { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(50)]
    public string? Province { get; set; }

    [StringLength(50)]
    public string? Country { get; set; }
}
