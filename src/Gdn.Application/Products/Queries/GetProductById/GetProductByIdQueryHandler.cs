using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Products.Queries.GetProductById;

internal sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<Product>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _productRepository = _unitOfWork.GetRepository<IProductRepository>();
    }

    public async Task<Result<Product>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _productRepository.GetAsync(request.Id);
        if (data is null)
            return ProductErrors.NotFound(request.Id);

        return data;
    }
}
