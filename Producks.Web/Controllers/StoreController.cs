using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Producks.Data;

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
            return View(await _context.Categories.ToListAsync());
        }

        public async Task<IActionResult> ProductsByCategory (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            var products = await _context.Products
                .Where(p => p.Category.Id == category.Id)
                .ToListAsync();
            

            return View(products);
        }
    }
}