using System.Collections.Generic;
using AutoMapper;
using WebApiProject.Domain.Entities;
using WebApiProject.Web.Models.Responses;

namespace WebApiProject.Web.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductResponseModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<IEnumerable<Product>, ProductsListResponseModel>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src));
        }
    }
}
