namespace Gdn.Application.ProductCategories;

public static class ProductCategoryErrors
{
    public static Error InvalidInput(string propertyName) => new("ProductCategory:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound() => new("ProductCategory:NotFound", "Product category not found");
}
