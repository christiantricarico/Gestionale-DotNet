namespace Gdn.Web.Api.Vs.Features.Payments;

public static class PaymentErrors
{
    public static Error NotFound(int id) => new("Payment:NotFound", $"Payment with Id={id} not found");
    public static Error DueNotFound(int id) => new("Payment:DueNotFound", $"Due with Id={id} not found");
    public static Error AllocationExceedsDue(int dueId) => new("Payment:AllocationExceedsDue", $"Allocation for due {dueId} would exceed its outstanding balance");
    public static Error AllocationExceedsPayment() => new("Payment:AllocationExceedsPayment", "Sum of allocations exceeds payment amount");
}
