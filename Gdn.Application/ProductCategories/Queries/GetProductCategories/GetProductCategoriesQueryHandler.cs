using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.ProductCategories.Queries.GetProductCategories;

internal sealed class GetProductCategoriesQueryHandler : IRequestHandler<GetProductCategoriesQuery, Result<IEnumerable<ProductCategory>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCategoryRepository _productCategoryRepository;

    public GetProductCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _productCategoryRepository = _unitOfWork.GetRepository<IProductCategoryRepository>();
    }

    public async Task<Result<IEnumerable<ProductCategory>>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var data = await _productCategoryRepository.GetAllAsync();
        return data.ToList();
    }
}
