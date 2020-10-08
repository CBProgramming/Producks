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
        bool CreateBrand(BrandModel brand);
        bool EditBrand(BrandModel brand);
        bool DetelteBrand(int? id);
    }
}
