using MediatR;

namespace Gdn.Application.ProductCategories.Commands.DeleteProductCategory;

public sealed record DeleteProductCatetoryCommand(int ProductCategoryId) : IRequest<Result<bool>>;
