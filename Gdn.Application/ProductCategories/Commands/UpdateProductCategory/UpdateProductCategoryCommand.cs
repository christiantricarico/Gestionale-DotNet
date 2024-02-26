using Gdn.Application.ProductCategories.Dtos;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.ProductCategories.Commands.UpdateProductCategory;

public sealed record UpdateProductCategoryCommand(ProductCategoryInput Input) : IRequest<Result<ProductCategory>>;
