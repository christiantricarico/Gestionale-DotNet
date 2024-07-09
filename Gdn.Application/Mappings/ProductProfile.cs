using AutoMapper;
using Gdn.Application.Products.Dtos;
using Gdn.Domain.Models;

namespace Gdn.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductInput, Product>();
    }
}
