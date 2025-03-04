using Gdn.Application.Products.Dtos;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(ProductInput Input) : IRequest<Result<Product>>;
