using AutoMapper;
using Gdn.Application.TaxRates.Dtos;
using Gdn.Domain.Models;

namespace Gdn.Application.Mappings;

public class TaxRateProfile : Profile
{
    public TaxRateProfile()
    {
        CreateMap<TaxRateInput, TaxRate>();
    }
}
