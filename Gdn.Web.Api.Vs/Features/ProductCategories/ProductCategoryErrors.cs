using Gdn.Application;

namespace Gdn.Web.Api.Vs.Features.ProductCategories;

public static class ProductCategoryErrors
{
    public static Error InvalidInput(string propertyName) => new("ProductCategory:InvalidInput", $"{propertyName} not valid");
    public static Error NotFound(int id) => new("ProductCategory:NotFound", $"Product category with Id={id} not found");
}
