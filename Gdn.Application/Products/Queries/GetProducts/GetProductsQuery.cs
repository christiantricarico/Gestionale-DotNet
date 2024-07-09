using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery : IRequest<Result<IEnumerable<Product>>>;
