using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Products.Commands.UpdateProduct;

internal sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<Product>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productRepository = _unitOfWork.GetRepository<IProductRepository>();
        _mapper = mapper;
    }

    public async Task<Result<Product>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var input = request.Input;

        if (!input.Id.HasValue)
            return ProductErrors.InvalidInput(nameof(input.Id));

        var entity = await _productRepository.GetAsync(input.Id.Value);
        if (entity is null)
            return ProductErrors.NotFound(input.Id.Value);

        _mapper.Map(input, entity);

        await _unitOfWork.SaveChangesAsync();

        return entity;
    }
}
