using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Products.Queries.GetProducts;

internal sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<IEnumerable<Product>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _productRepository = _unitOfWork.GetRepository<IProductRepository>();
    }

    public async Task<Result<IEnumerable<Product>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        List<string> includes = new();

        if (request.IncludeProductCategory)
            includes.Add("ProductCategory");

        var data = await _productRepository.GetAllAsync(includes);
        return data.ToList();
    }
}
