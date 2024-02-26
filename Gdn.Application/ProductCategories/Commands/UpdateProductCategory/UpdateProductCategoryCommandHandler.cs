using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.ProductCategories.Commands.UpdateProductCategory;

internal sealed class UpdateProductCategoryCommandHandler : IRequestHandler<UpdateProductCategoryCommand, Result<ProductCategory>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly IMapper _mapper;

    public UpdateProductCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productCategoryRepository = _unitOfWork.GetRepository<IProductCategoryRepository>();
        _mapper = mapper;
    }

    public async Task<Result<ProductCategory>> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var input = request.Input;

        var entity = await _productCategoryRepository.GetAsync(request.Input.Id);
        if (entity is null)
            return ProductCategoryErrors.NotFound;

        _mapper.Map(input, entity);

        await _unitOfWork.SaveChangesAsync();

        return entity;
    }
}
