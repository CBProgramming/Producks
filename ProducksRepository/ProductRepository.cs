using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Producks.Data;
using ProducksRepository.Models;

namespace ProducksRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreDb _context;
        private readonly IMapper _mapper;

        public ProductRepository(StoreDb context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CreateProduct(ProductModel product)
        {
            try
            {
                _context.Add(_mapper.Map<Product>(product));
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> DetelteProduct(int? id)
        {
            var product = await _context.Products.FindAsync(id);
            product.Active = false;
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> EditProduct(ProductModel product)
        {
            try
            {
                _context.Update(_mapper.Map<Product>(product));
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public ProductModel GetProduct(int? id)
        {
            return _mapper.Map<Product, ProductModel>(_context.Products
                .Where(b => b.Active == true)
                .Include("Category")
                .Include("Brand")
                .FirstOrDefaultAsync(b => b.Id == id)
                .Result);
        }

        public List<ProductModel> GetProducts()
        {
            var products = _context.Products
                           .Where(b => b.Active == true)
                           .Include("Category")
                           .Include("Brand")
                           .AsEnumerable()
                           .Select(b => _mapper.Map<Product, ProductModel>(b))
                           .ToList();
            return products;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(b => b.Id == id);
        }
    }
}
