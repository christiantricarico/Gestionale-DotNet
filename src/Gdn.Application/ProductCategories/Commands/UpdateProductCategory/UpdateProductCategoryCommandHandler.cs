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

        if (!input.Id.HasValue)
            return ProductCategoryErrors.InvalidInput(nameof(input.Id));

        var entity = await _productCategoryRepository.GetAsync(input.Id.Value);
        if (entity is null)
            return ProductCategoryErrors.NotFound(input.Id.Value);

        _mapper.Map(input, entity);

        await _unitOfWork.SaveChangesAsync();

        return entity;
    }
}
