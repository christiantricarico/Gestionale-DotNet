namespace Gdn.Web.Api.Vs.Features.PaymentMethods;

public static class PaymentMethodErrors
{
    public static Error NotFound(int id) => new("PaymentMethod:NotFound", $"PaymentMethod with Id={id} not found");
}
