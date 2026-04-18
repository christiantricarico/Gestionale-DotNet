namespace Gdn.Web.Api.Vs.Features.Payments;

public static class PaymentErrors
{
    public static Error NotFound(int id) => new("Payment:NotFound", $"Payment with Id={id} not found");
    public static Error DueNotFound(int id) => new("Payment:DueNotFound", $"Due with Id={id} not found");
    public static Error AllocationSignMismatch(int dueId) => new("Payment:AllocationSignMismatch", $"Allocation for due {dueId} has an invalid sign");
    public static Error AllocationExceedsDue(int dueId) => new("Payment:AllocationExceedsDue", $"Allocation for due {dueId} would exceed its outstanding balance");
    public static Error AllocationExceedsPayment() => new("Payment:AllocationExceedsPayment", "Sum of allocations exceeds payment amount");
    public static Error CustomerNotFound(int id) => new("Payment:CustomerNotFound", $"Customer with Id={id} not found");
    public static Error DueNotBelongToCustomer(int dueId, int customerId) => new("Payment:DueNotBelongToCustomer", $"Due with Id={dueId} does not belong to customer {customerId}");
}
