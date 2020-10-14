using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Producks.Data;
using ProducksRepository.Models;

namespace ProducksRepository
{
    class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<BrandModel, Brand>();
            CreateMap<Brand, BrandModel>();
            CreateMap<BrandModel, Brand>();
            CreateMap<Brand, BrandModel>()
                .ForMember(dest => dest.Active, opt => opt.Ignore());
        }
    }
}
