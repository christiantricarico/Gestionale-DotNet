using AutoMapper;
using Gdn.Application.Invoices.Dtos;
using Gdn.Domain.Models;

namespace Gdn.Application.Mappings;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<InvoiceInput, Invoice>();
    }
}
