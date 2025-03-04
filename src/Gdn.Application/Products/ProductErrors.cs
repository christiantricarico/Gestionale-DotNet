namespace Gdn.Application.Products;

public static class ProductErrors
{
    public static Error InvalidInput(string propertyName) => new("Product:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound(int id) => new("Product:NotFound", $"Product with Id={id} not found");
}
