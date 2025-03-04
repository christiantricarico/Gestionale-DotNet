using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.ProductCategories.Queries.GetProductCategoryById;

public sealed record GetProductCategoryByIdQuery(int Id) : IRequest<Result<ProductCategory>>;
