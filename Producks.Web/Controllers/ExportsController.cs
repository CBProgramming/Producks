using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Producks.Data;
using Producks.Web.Models;

namespace Producks.Web.Controllers
{
    [ApiController]
    public class ExportsController : ControllerBase
    {
        private readonly StoreDb _context;

        public ExportsController(StoreDb context)
        {
            _context = context;
        }

        // GET: api/Brands
        [HttpGet("api/Brands")]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _context.Brands
                                       .Select(b => new BrandDto
                                       {
                                           Id = b.Id,
                                           Name = b.Name,
                                           Active = b.Active
                                       })
                                       .ToListAsync();
            return Ok(brands);
        }

        // GET: api/Categories
        [HttpGet("api/Categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();
            return Ok(categories);
        }

        // GET api/Products by category
        [HttpGet("api/ProductsByCategory")]
        public async Task<IActionResult> GetProductsByCategory(string category, string brand, int? priceLow, int? priceHigh)
        {
            var products = await _context.Products.Select(p => new ProductDto
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                BrandId = p.BrandId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockLevel = p.StockLevel,
                Category = p.Category,
                Brand = p.Brand
            })
                
                .ToListAsync();
            if (category != null)
            {
                products = products.Where(p => p.Category.ToString().Equals(category)).ToList();
            }
            if (brand != null)
            {
                products = products.Where(p => p.Brand.ToString().Equals(brand)).ToList();
            }
            if (priceLow > 0)
            {
                products = products.Where(p => p.Price >= priceLow).ToList();
            }
            if (priceHigh > 0)
            {
                products = products.Where(p => p.Price <= priceHigh).ToList();
            }

            return Ok(products);
        }

    }


}
