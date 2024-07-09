using AutoMapper;
using Gdn.Application.Products.Dtos;
using Gdn.Domain.Models;
using Gdn.Presentation.Shared.Models.Products;

namespace Gdn.Web.Api.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductInputModel, ProductInput>();
        CreateMap<Product, ProductViewModel>();
    }
}
