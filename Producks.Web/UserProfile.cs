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
            CreateMap<ProducksRepository.Models.BrandModel, BrandVM>();
            CreateMap<BrandVM, ProducksRepository.Models.BrandModel>();

            CreateMap<BrandModel, Brand>();
            CreateMap<Brand, BrandModel>()
                .ForMember(dest => dest.Active, opt => opt.Ignore());
        }
    }
}
