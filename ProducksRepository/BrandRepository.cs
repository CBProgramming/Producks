using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Producks.Data;
using ProducksRepository.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace ProducksRepository
{
    public class BrandRepository : IBrandRepository
    {

        private readonly StoreDb _context;
        private readonly IMapper _mapper;


        public BrandRepository(StoreDb context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CreateBrand(BrandModel brand)
        {
            try
            {
                _context.Add(_mapper.Map<Brand>(brand));
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
                _context.Update(_mapper.Map<Brand>(brand));
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

        public BrandModel GetBrand(int? id)
        {
            return _mapper.Map<Brand, BrandModel>(_context.Brands
                .Where(b => b.Active == true)
                .FirstOrDefaultAsync(b => b.Id == id)
                .Result);
        }

        public List<BrandModel> GetBrands()
        {
            var brands = _context.Brands
                           .Where(b => b.Active == true)
                           .AsEnumerable()
                           .Select(b => _mapper.Map<Brand, BrandModel>(b))
                           .ToList();
            return brands;
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(b => b.Id == id);
        }
    }
}
