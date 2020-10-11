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

        public async Task<bool> CreateBrand(BrandModel brand)
        {
            try
            {
                _context.Add(generateBrand(brand));
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> DetelteBrand(int? id)
        {
            var brand = await _context.Brands.FindAsync(id);
            brand.Active = false;
            try
            {
                _context.Update(brand);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(brand.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> EditBrand(BrandModel brand)
        {
            try
            {
                _context.Update(generateBrand(brand));
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(brand.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public Task<BrandModel> GetBrand(int? id)
        {
            var brand = _context.Brands
                .Where(b => b.Active == true)
                .Select(b => new BrandModel
                {
                    Id = b.Id,
                    Name = b.Name
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

        public Brand generateBrand(BrandModel brand)
        {
            return new Brand
            {
                Id = brand.Id,
                Name = brand.Name,
                Active = true
            };
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(b => b.Id == id);
        }
    }
}
