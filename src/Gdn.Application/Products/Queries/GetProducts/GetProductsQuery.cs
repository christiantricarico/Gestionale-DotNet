using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    bool IncludeProductCategory = false)
    : IRequest<Result<IEnumerable<Product>>>;
