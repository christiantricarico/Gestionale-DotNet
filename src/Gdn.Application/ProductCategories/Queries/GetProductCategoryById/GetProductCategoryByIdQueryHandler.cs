using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.ProductCategories.Queries.GetProductCategoryById;

internal sealed class GetProductCategoryByIdQueryHandler : IRequestHandler<GetProductCategoryByIdQuery, Result<ProductCategory>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCategoryRepository _productCategoryRepository;

    public GetProductCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _productCategoryRepository = _unitOfWork.GetRepository<IProductCategoryRepository>();
    }

    public async Task<Result<ProductCategory>> Handle(GetProductCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _productCategoryRepository.GetAsync(request.Id);
        if (data is null)
            return ProductCategoryErrors.NotFound(request.Id);

        return data;
    }
}
