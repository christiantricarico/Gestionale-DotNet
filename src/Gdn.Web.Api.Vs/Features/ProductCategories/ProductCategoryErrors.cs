namespace Gdn.Web.Api.Vs.Features.ProductCategories;

public static class ProductCategoryErrors
{
    public static Error InvalidInput(string propertyName) => new("ProductCategory:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound(int id) => new("ProductCategory:NotFound", $"Product category with Id={id} not found");
    public static Error ParentNotFound(int parentId) => new("ProductCategory:ParentNotFound", $"Parent product category with Id={parentId} not found");
    public static Error CircularReference(int parentId) => new("ProductCategory:CircularReference", $"Parent product category with Id={parentId} creates a circular reference");
}
