using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.ProductCategories.Queries.GetProductCategories;

public sealed record GetProductCategoriesQuery : IRequest<Result<IEnumerable<ProductCategory>>>;
