using AutoMapper;
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
        }
    }
}
