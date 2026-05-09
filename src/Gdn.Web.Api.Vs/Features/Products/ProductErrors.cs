namespace Gdn.Web.Api.Vs.Features.Products;

public static class ProductErrors
{
    public static Error InvalidInput(string propertyName) => new("Product:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound(int id) => new("Product:NotFound", $"Product with Id={id} not found");
    public static Error CodeAlreadyExists(string code) => new("Product:CodeAlreadyExists", $"Product with Code={code} already exists");
}
