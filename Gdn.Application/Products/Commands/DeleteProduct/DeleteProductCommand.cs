using MediatR;

namespace Gdn.Application.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(int ProductId) : IRequest<Result<bool>>;
