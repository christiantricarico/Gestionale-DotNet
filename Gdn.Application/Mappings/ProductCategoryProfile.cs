using AutoMapper;
using Gdn.Application.ProductCategories.Commands.Dtos;
using Gdn.Domain.Models;

namespace Gdn.Application.Mappings;

public class ProductCategoryProfile : Profile
{
    public ProductCategoryProfile()
    {
        CreateMap<ProductCategoryInput, ProductCategory>();
    }
}
