using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Products.Commands.CreateProduct;

internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Product>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork,
                                       IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productRepository = _unitOfWork.GetRepository<IProductRepository>();
        _mapper = mapper;
    }

    public async Task<Result<Product>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var input = request.Input;

        var entity = _mapper.Map<Product>(input);

        _productRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity;
    }
}
