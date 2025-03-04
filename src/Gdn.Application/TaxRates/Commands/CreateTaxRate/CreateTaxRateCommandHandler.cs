using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.TaxRates.Commands.CreateTaxRate;

internal sealed class CreateTaxRateCommandHandler : IRequestHandler<CreateTaxRateCommand, Result<TaxRate>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaxRateRepository _taxRateRepository;
    private readonly IMapper _mapper;


    public CreateTaxRateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _taxRateRepository = _unitOfWork.GetRepository<ITaxRateRepository>();

        _mapper = mapper;
    }


    public async Task<Result<TaxRate>> Handle(CreateTaxRateCommand request, CancellationToken cancellationToken)
    {
        var input = request.Input;

        var entity = _mapper.Map<TaxRate>(input);

        _taxRateRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity;
    }
}
