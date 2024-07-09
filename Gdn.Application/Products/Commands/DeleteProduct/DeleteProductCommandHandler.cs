using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using MediatR;

namespace Gdn.Application.Products.Commands.DeleteProduct;

internal sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork,
                                       IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productRepository = _unitOfWork.GetRepository<IProductRepository>();
        _mapper = mapper;
    }

    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _productRepository.GetAsync(request.ProductId);
        if (entity is null)
            return ProductErrors.NotFound(request.ProductId);

        entity.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
