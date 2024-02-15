namespace Gdn.Application.ProductCategories.Commands.Dtos;

public sealed record ProductCategoryInput(string Code,
                                          string? Name,
                                          string? Description,
                                          int Level,
                                          int? ParentCategoryId);
