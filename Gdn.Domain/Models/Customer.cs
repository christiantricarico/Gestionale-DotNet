using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public class Customer : RegistryEntity<int>
{
    public string? FiscalCode { get; set; }
    public string? VatNumber { get; set; }
}
