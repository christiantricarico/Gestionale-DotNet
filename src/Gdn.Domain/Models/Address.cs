using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

public class Address : TrackedEntity<int>
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
}
