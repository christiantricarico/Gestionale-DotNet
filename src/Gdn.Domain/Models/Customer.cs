using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public class Customer : RegistryEntity<int>
{
    public string? FiscalCode { get; set; }
    public string? VatNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Pec { get; set; }
    public string? Sdi { get; set; }
    public string? Notes { get; set; }

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
