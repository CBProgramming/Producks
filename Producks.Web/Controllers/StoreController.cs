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
using Producks.UndercuttersFacade;
using Producks.UndercuttersFacade.Models;
using Producks.Web.Models;

namespace Producks.Web.Controllers
{
    public class StoreController : Controller

    {

        private readonly StoreDb _context;
        private readonly IConfiguration _configuration;
        private readonly ICategory _category;
        private HttpClient undercuttersClient;
        private IEnumerable<ProductDtoUndercutters> undercuttersProducts;
        private IEnumerable<BrandDtoUndercutters> undercuttersBrands;
        private List<CategoryDtoUndercutters> undercuttersCategories;
        private IEnumerable<ProductVM> localProducts;
        private IEnumerable<BrandVM> localBrands;
        private IEnumerable<CategoryVM> localCategories;
        private List<ProductVM> mergedProducts;
        private List<BrandVM> mergedBrands;
        private List<CategoryVM> mergedCategories;

        public StoreController(StoreDb context, IConfiguration configuration, ICategory category)
        {
            _context = context;
            _configuration = configuration;
            _category = category;
            setupUndercutersClient();
        }

        public async Task<IActionResult> Index()
        {
            //pull brands, categories and products and combine to display with local stuff
            //make some DTOs
            localBrands = await generateLocalBrands();
            undercuttersBrands = await generateUndercuttersBrands();
            undercuttersBrands = undercuttersBrands.ToList();
            mergeBrands();
            localCategories = await generateLocalCategories();
            undercuttersCategories = await _category.GetCategories();
            mergeCategories();
            StoreIndexVM storeIndex = new StoreIndexVM
            {
                Categories = mergedCategories.ToList(),
                Brands = mergedBrands.ToList()
            };
            return View(storeIndex);
        }

        public async Task<IActionResult> ProductsByCategory (string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            localProducts = await generateLocalProducts();
            localProducts = localProducts.Where(p => p.Category.Name.Equals(id));
            undercuttersProducts = await generateUndercuttersProductsByCategory(id);
            mergeProducts();
            return View(mergedProducts);
        }

        public async Task<IActionResult> ProductsByBrand(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            localProducts = await generateLocalProducts();
            localProducts = localProducts.Where(p => p.Brand.Name.Equals(id));
            undercuttersProducts = await generateUndercuttersProductsByBrand(id);
            mergeProducts();
            return View(mergedProducts);
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

        public Producks.Data.Category generateCategory(CategoryVM category)
        {
            return new Producks.Data.Category
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

        public async Task<IEnumerable<ProductVM>> generateLocalProducts()
        {
            return await _context.Products
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
                .Where(p => p.Active == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductDtoUndercutters>> generateUndercuttersProducts()
        {
            string uri = "/api/Product";
            try
            {
                var response = await undercuttersClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<ProductDtoUndercutters>>();
            }
            catch (HttpRequestException e)
            {
                return Array.Empty<ProductDtoUndercutters>();
            }
        }

        public async Task<IEnumerable<ProductDtoUndercutters>> generateUndercuttersProductsByCategory(string categoryName)
        {
            string uri = "/api/Product?category_name=" + categoryName;
            try
            {
                var response = await undercuttersClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<ProductDtoUndercutters>>();
            }
            catch (HttpRequestException e)
            {
                return Array.Empty<ProductDtoUndercutters>();
            }
        }

        public async Task<IEnumerable<ProductDtoUndercutters>> generateUndercuttersProductsByBrand(string brandName)
        {
            int categoryId = -1;
            undercuttersBrands = await generateUndercuttersBrands();
            undercuttersBrands = undercuttersBrands.ToList();
            foreach (BrandDtoUndercutters brand in undercuttersBrands)
            {
                if (brand.Name.Equals(brandName))
                {
                    categoryId = brand.Id;
                }
            }
            if (categoryId != -1)
            {
                string uri = "/api/Product?brand_id=" + categoryId;
                try
                {
                    var response = await undercuttersClient.GetAsync(uri);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<List<ProductDtoUndercutters>>();
                }
                catch (HttpRequestException e)
                {
                    return Array.Empty<ProductDtoUndercutters>();
                }
            }
            return Array.Empty<ProductDtoUndercutters>();
        }

        private void mergeCategories()
        {
            mergedCategories = new List<CategoryVM>();
            List<String> alreadyAdded = new List<String>();
            foreach (CategoryVM category in localCategories)
            {
                if (!alreadyAdded.Contains(category.Name))
                {
                    mergedCategories.Add(category);
                    alreadyAdded.Add(category.Name);
                }
            }
            foreach (CategoryDtoUndercutters undercuttersCategory in undercuttersCategories)
            {
                if (!alreadyAdded.Contains(undercuttersCategory.Name))
                {
                    mergedCategories.Add(new CategoryVM
                    {
                        Id = 0,
                        Name = undercuttersCategory.Name,
                        Description = undercuttersCategory.Description
                    });
                    alreadyAdded.Add(undercuttersCategory.Name);
                }
            }
            mergedCategories.OrderBy(b => b.Name);
        }

        private void mergeBrands()
        {
            mergedBrands = new List<BrandVM>();
            List<String> alreadyAdded = new List<String>();
            foreach (BrandVM brand in localBrands)
            {
                if (!alreadyAdded.Contains(brand.Name))
                {
                    mergedBrands.Add(brand);
                    alreadyAdded.Add(brand.Name);
                }
            }
            foreach (BrandDtoUndercutters undercuttersBrand in undercuttersBrands)
            {
                if (!alreadyAdded.Contains(undercuttersBrand.Name))
                {
                    mergedBrands.Add(new BrandVM
                    {
                        Id = undercuttersBrand.Id,
                        Name = undercuttersBrand.Name,
                        Active = true
                    });
                    alreadyAdded.Add(undercuttersBrand.Name);
                }
            }
            mergedBrands.OrderBy(b => b.Name);
        }

        private void mergeProducts()
        {
            mergedProducts = new List<ProductVM>();
            foreach (ProductVM product in localProducts)
            {
                mergedProducts.Add(product);
            }
            foreach (ProductDtoUndercutters undercuttersProduct in undercuttersProducts)
            {
                mergedProducts.Add(new ProductVM
                {
                    Id = undercuttersProduct.Id,
                    CategoryId = undercuttersProduct.CategoryId,
                    BrandId = undercuttersProduct.BrandId,
                    Name = undercuttersProduct.Name,
                    Description = undercuttersProduct.Description,
                    Price = undercuttersProduct.Price,
                    StockLevel = undercuttersProduct.StockLevel,
                    Active = true,
                    //Category = undercuttersProduct.Category,
                    //Brand = undercuttersProduct.Brand
                });
            }
            mergedProducts.OrderBy(b => b.Name);
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
                return new List<CategoryDtoUndercutters> ();
            }
        }
    }
}