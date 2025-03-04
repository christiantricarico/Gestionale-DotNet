using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Invoices.Queries.GetInvoiceById;

internal sealed class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, Result<Invoice>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.GetRepository<IInvoiceRepository>();
    }

    public async Task<Result<Invoice>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _invoiceRepository.GetAsync(request.Id);
        if (data is null)
            return InvoiceErrors.NotFound(request.Id);

        return data;
    }
}
