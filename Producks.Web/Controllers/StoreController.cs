using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Producks.Data;
using Producks.Web.Models;

namespace Producks.Web.Controllers
{
    public class StoreController : Controller

    {

        private readonly StoreDb _context;
        private readonly IConfiguration _configuration;
        private HttpClient undercuttersClient;
        private IEnumerable<ProductDtoUndercutters> undercuttersProducts;
        private IEnumerable<BrandDtoUndercutters> undercuttersBrands;
        private IEnumerable<CategoryDtoUndercutters> undercuttersCategories;
        private IEnumerable<ProductVM> localProducts;
        private IEnumerable<BrandVM> localBrands;
        private IEnumerable<CategoryVM> localCategories;
        private IEnumerable<ProductVM> mergedProducts;
        private List<BrandVM> mergedBrands;
        private List<CategoryVM> mergedCategories;

        public StoreController(StoreDb context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            setupUndercutersClient();
        }

        public async Task<IActionResult> Index()
        {
            //pull brands, categories and products and combine to display with local stuff
            //make some DTOs
            localBrands = await generateLocalBrands();
            undercuttersBrands = await generateUndercuttersBrands();
            mergeBrands();
            localCategories = await generateLocalCategories();
            undercuttersCategories = await generateUndercuttersCategories();
            mergeCategories();
            StoreIndexVM storeIndex = new StoreIndexVM
            {
                Categories = mergedCategories.ToList(),
                Brands = mergedBrands.ToList()
            };
            return View(storeIndex);
        }

        private void mergeCategories()
        {
            mergedCategories = new List<CategoryVM>();
            foreach (CategoryVM category in localCategories)
            {
                mergedCategories.Add(category);
            }
            foreach (CategoryDtoUndercutters undercuttersCategory in undercuttersCategories)
            {
                mergedCategories.Add(new CategoryVM
                {
                    Id = undercuttersCategory.Id,
                    Name = undercuttersCategory.Name,
                    Description = undercuttersCategory.Description
                });
            }
            mergedCategories.OrderBy(b => b.Name);
        }

        private void mergeBrands()
        {
            mergedBrands = new List<BrandVM>();
            foreach (BrandVM brand in localBrands)
            {
                mergedBrands.Add(brand);
            }
            foreach (BrandDtoUndercutters undercuttersBrand in undercuttersBrands)
            {
                mergedBrands.Add(new BrandVM
                {
                    Id = undercuttersBrand.Id,
                    Name = undercuttersBrand.Name,
                    Active = true
                });
            }
            mergedBrands.OrderBy(b => b.Name);
        }

        private async Task<IEnumerable<CategoryVM>> generateLocalCategories()
        {
            return await _context.Categories
               .Select(c => new CategoryVM
               {
                   Id = c.Id,
                   Name = c.Name,
                   Description = c.Description,
                   Active = c.Active
               })
               .Where(b => b.Active == true)
               .ToListAsync();
        }

        private async Task<IEnumerable<BrandVM>> generateLocalBrands()
        {
            return await _context.Brands
               .Select(b => new BrandVM
               {
                   Id = b.Id,
                   Name = b.Name,
                   Active = b.Active
               })
               .Where(b => b.Active == true)
               .ToListAsync();
        }

        public async Task<IEnumerable<BrandDtoUndercutters>> generateUndercuttersBrands()
        {
            string uri = "/api/Brand";
            try
            {
                var response = await undercuttersClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<BrandDtoUndercutters>>();
            }
            catch (HttpRequestException e)
            {
                return Array.Empty<BrandDtoUndercutters>();
            }
        }

        public async void generateUndercuttersProducts()
        {
            string uri = "/api/Product";
            try
            {
                var response = await undercuttersClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                undercuttersProducts = await response.Content.ReadAsAsync<List<ProductDtoUndercutters>>();
            }
            catch (HttpRequestException e)
            {
                undercuttersProducts = Array.Empty<ProductDtoUndercutters>();
            }
        }



        public async Task<IEnumerable<CategoryDtoUndercutters>> generateUndercuttersCategories()
        {
            string uri = "/api/Category";
            try
            {
                var response = await undercuttersClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<CategoryDtoUndercutters>>();
            }
            catch (HttpRequestException e)
            {
                return undercuttersCategories = Array.Empty<CategoryDtoUndercutters>();
            }
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

        public async Task<IActionResult> ProductsByBrand(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            BrandVM brand = getBrandVM(id).Result;
            if (brand == null)
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
                .Where(p => p.Brand.Id == brand.Id)
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

        public async Task<BrandVM> getBrandVM(int? id)
        {
            return await _context.Brands
                .Select(b => new BrandVM
                {
                    Id = b.Id,
                    Name = b.Name,
                    Active = b.Active
                })
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        private void setupUndercutersClient()
        {
            undercuttersClient = new HttpClient();
            undercuttersClient.BaseAddress = new Uri(_configuration["UndercuttersBaseUri"]);
            undercuttersClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            undercuttersClient.Timeout = TimeSpan.FromSeconds(5);
        }
    }
}