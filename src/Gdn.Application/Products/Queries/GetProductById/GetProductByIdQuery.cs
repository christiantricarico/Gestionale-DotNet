using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(int Id) : IRequest<Result<Product>>;
