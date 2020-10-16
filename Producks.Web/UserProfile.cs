using AutoMapper;
using Producks.Data;
using Producks.Web.Models;
using ProducksRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Producks.Web
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<BrandModel, BrandVM>();
            CreateMap<BrandVM, BrandModel>();

            CreateMap<BrandModel, Brand>();
            CreateMap<Brand, BrandModel>()
                .ForMember(dest => dest.Active, opt => opt.Ignore());

            CreateMap<CategoryModel, CategoryVM>();
            CreateMap<CategoryVM, CategoryModel>();
            CreateMap<CategoryModel, Category>();
            CreateMap<Category, CategoryModel>();

            CreateMap<ProductModel, ProductVM>();
            CreateMap<ProductVM, ProductModel>();
            CreateMap<ProductModel, Product>();
            CreateMap<Product, ProductModel>();
            CreateMap<List<ProductModel>, List<ProductVM>>();
            CreateMap<List<ProductVM>, List<ProductModel>>();
        }
    }
}
