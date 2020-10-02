using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Producks.Data;
using Producks.Web.Models;

namespace Producks.Web.Controllers
{
    public class StoreController : Controller
    {

        private readonly StoreDb _context;

        public StoreController(StoreDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
               .Select(c => new CategoryVM
               {
                   Id = c.Id,
                   Name = c.Name,
                   Description = c.Description,
                   Active = c.Active
               })
               .Where(b => b.Active == true)
               .ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> ProductsByCategory (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            CategoryVM category = getCategoryVM(id).Result;
            if (category == null)
            {
                return NotFound();
            }
            var products = await _context.Products
                .Select(p => new ProductVM
                {
                    Id = p.Id,
                    CategoryId = p.CategoryId,
                    BrandId = p.BrandId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockLevel = p.StockLevel,
                    Active = p.Active,
                    Category = p.Category,
                    Brand = p.Brand
                })
                .Where(p => p.Category.Id == category.Id)
                .Where(p => p.Active == true)
                .ToListAsync();
            return View(products);
        }

        public async Task<CategoryVM> getCategoryVM(int? id)
        {
            return await _context.Categories
                .Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Active = c.Active
                })
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public Category generateCategory(CategoryVM category)
        {
            return new Category
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Active = category.Active
            };
        }
    }
}