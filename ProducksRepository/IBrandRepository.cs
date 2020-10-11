using ProducksRepository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProducksRepository
{
    public interface IBrandRepository
    {
        Task<List<BrandModel>> GetBrands();
        Task<BrandModel> GetBrand(int? id);
        Task<bool> CreateBrand(BrandModel brand);
        Task<bool> EditBrand(BrandModel brand);
        Task<bool> DetelteBrand(int? id);
    }
}
