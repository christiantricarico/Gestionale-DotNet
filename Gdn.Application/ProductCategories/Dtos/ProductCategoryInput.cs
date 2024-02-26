namespace Gdn.Application.ProductCategories.Dtos;

public sealed record ProductCategoryInput(
    int Id,
    string Code,
    string? Name,
    string? Description,
    int Level,
    int? ParentCategoryId);
