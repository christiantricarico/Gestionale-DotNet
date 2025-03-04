using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using MediatR;

namespace Gdn.Application.ProductCategories.Commands.DeleteProductCategory;

internal sealed class DeleteProductCategoryCommandHandler : IRequestHandler<DeleteProductCatetoryCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly IMapper _mapper;

    public DeleteProductCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productCategoryRepository = _unitOfWork.GetRepository<IProductCategoryRepository>();
        _mapper = mapper;
    }

    public async Task<Result<bool>> Handle(DeleteProductCatetoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _productCategoryRepository.GetAsync(request.ProductCategoryId);
        if (entity is null)
            return ProductCategoryErrors.NotFound(request.ProductCategoryId);

        entity.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
