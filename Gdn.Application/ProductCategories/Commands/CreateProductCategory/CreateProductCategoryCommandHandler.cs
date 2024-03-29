﻿using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.ProductCategories.Commands.CreateProductCategory;

internal sealed class CreateProductCategoryCommandHandler : IRequestHandler<CreateProductCategoryCommand, Result<ProductCategory>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProductCategoryRepository _productCategoryRepository;

    public CreateProductCategoryCommandHandler(IUnitOfWork unitOfWork,
                                               IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productCategoryRepository = _unitOfWork.GetRepository<IProductCategoryRepository>();
        _mapper = mapper;
    }

    public async Task<Result<ProductCategory>> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var input = request.Input;

        var entity = _mapper.Map<ProductCategory>(input);

        await _productCategoryRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity;
    }
}
