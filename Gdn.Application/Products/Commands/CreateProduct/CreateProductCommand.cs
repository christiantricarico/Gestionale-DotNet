using Gdn.Application.Products.Dtos;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(ProductInput Input) : IRequest<Result<Product>>;
