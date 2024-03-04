using AutoMapper;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.TaxRates.Commands.UpdateTaxRate;

internal sealed class UpdateTaxRateCommandHandler : IRequestHandler<UpdateTaxRateCommand, Result<TaxRate>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITaxRateRepository _taxRateRepository;
    private readonly IMapper _mapper;

    public UpdateTaxRateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _taxRateRepository = _unitOfWork.GetRepository<ITaxRateRepository>();

        _mapper = mapper;
    }


    public async Task<Result<TaxRate>> Handle(UpdateTaxRateCommand request, CancellationToken cancellationToken)
    {
        var input = request.Input;

        if (!input.Id.HasValue)
            return TaxRateErrors.InvalidInput(nameof(input.Id));

        var entity = await _taxRateRepository.GetAsync(input.Id.Value);
        if (entity is null)
            return TaxRateErrors.NotFound(input.Id.Value);

        _mapper.Map(input, entity);

        await _unitOfWork.SaveChangesAsync();

        return entity;
    }
}
