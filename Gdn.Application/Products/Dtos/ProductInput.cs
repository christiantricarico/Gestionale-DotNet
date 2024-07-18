namespace Gdn.Application.Products.Dtos;

public sealed class ProductInput
{
    // init-only properties in order to avoid value changes in command-handlers

    public int? Id { get; init; }
    public required string Code { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public int? ProductCategoryId { get; init; }
    public int? TaxRateId { get; init; }
}
