namespace Gdn.Application.ProductCategories.Dtos;

public record ProductCategoryInput(
    int Id,
    string Code,
    string? Name,
    string? Description,
    int Level,
    int? ParentCategoryId);
