namespace Gdn.Application.ProductCategories.Dtos;

public class ProductCategoryInput
{
    public int? Id { get; init; }
    public required string Code { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public int? ParentCategoryId { get; init; }
}