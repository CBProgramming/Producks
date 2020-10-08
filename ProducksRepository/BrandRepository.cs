using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Producks.Data;
using ProducksRepository.Models;
using Microsoft.EntityFrameworkCore;

namespace ProducksRepository
{
    public class BrandRepository : IBrandRepository
    {

        private readonly StoreDb _context;

        public BrandRepository(StoreDb context)
        {
            _context = context;
        }

        public bool CreateBrand(BrandModel brand)
        {
            throw new NotImplementedException();
        }

        public bool DetelteBrand(int? id)
        {
            throw new NotImplementedException();
        }

        public bool EditBrand(BrandModel brand)
        {
            throw new NotImplementedException();
        }

        public Task<BrandModel> GetBrand(int? id)
        {
            var brand = _context.Brands
                .Select(b => new BrandModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    Active = b.Active
                })
                .FirstOrDefaultAsync(b => b.Id == id);
            return brand;
        }

        public Task<List<BrandModel>> GetBrands()
        {
            var brands = _context.Brands
                           .Where(b => b.Active == true)
                           .Select(b => new BrandModel
                           {
                               Id = b.Id,
                               Name = b.Name
                           })
                           .ToListAsync();
            return brands;
        }
    }
}
