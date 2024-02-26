using Gdn.Application.ProductCategories.Dtos;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.ProductCategories.Commands.CreateProductCategory;

public sealed record CreateProductCategoryCommand(ProductCategoryInput Input) : IRequest<Result<ProductCategory>>;


