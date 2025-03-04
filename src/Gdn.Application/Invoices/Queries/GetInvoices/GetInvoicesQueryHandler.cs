using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using MediatR;

namespace Gdn.Application.Invoices.Queries.GetInvoices;

internal sealed class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, Result<IEnumerable<Invoice>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoicesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = _unitOfWork.GetRepository<IInvoiceRepository>();
    }

    public async Task<Result<IEnumerable<Invoice>>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        var data = await _invoiceRepository.GetAllAsync();
        return data.ToList();
    }
}
