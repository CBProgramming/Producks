using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Producks.Data;
using Producks.UndercuttersFacade;
using Producks.UndercuttersFacade.Models;
using Producks.Web.Models;
using ProducksRepository;
using ProducksRepository.Models;

namespace Producks.Web.Controllers
{
    public class StoreController : Controller

    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ICategoryUndercutters undercuttersCategoryGetter;
        private readonly IBrandUndercutters undercuttersBrandGetter;
        private readonly IProductUndercutters undercuttersProductGetter;
        private HttpClient undercuttersClient;
        private IEnumerable<ProductDtoUndercutters> undercuttersProducts;
        private List<BrandDtoUndercutters> undercuttersBrands;
        private List<CategoryDtoUndercutters> undercuttersCategories;
        private IEnumerable<ProductVM> localProducts;
        private IEnumerable<BrandVM> localBrands;
        private IEnumerable<CategoryVM> localCategories;
        private List<ProductVM> mergedProducts;
        private List<BrandVM> mergedBrands;
        private List<CategoryVM> mergedCategories;

        public StoreController(StoreDb context,
            IConfiguration configuration,
            ICategoryUndercutters _undercuttersCategoryGetter,
            IBrandUndercutters _undercuttersBrandGetter,
            IProductUndercutters _undercuttersProductGetter,
            IProductRepository productRepository, IBrandRepository brandRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _configuration = configuration;
            undercuttersCategoryGetter = _undercuttersCategoryGetter;
            undercuttersBrandGetter = _undercuttersBrandGetter;
            undercuttersProductGetter = _undercuttersProductGetter;
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            setupUndercutersClient();
        }

        public async Task<IActionResult> Index()
        {
            //pull brands, categories and products and combine to display with local stuff
            //make some DTOs
            localBrands = _mapper.Map<List<BrandVM>>(await _brandRepository.GetBrands());
            undercuttersBrands = await undercuttersBrandGetter.GetBrands();
            //undercuttersBrands = undercuttersBrands.ToList();
            mergeBrands();
            localCategories = _mapper.Map<List<CategoryVM>>(await _categoryRepository.GetCategories());
            undercuttersCategories = await undercuttersCategoryGetter.GetCategories();
            mergeCategories();
            StoreIndexVM storeIndex = new StoreIndexVM
            {
                Categories = mergedCategories.ToList(),
                Brands = mergedBrands.ToList()
            };
            return View(storeIndex);
        }

        public async Task<IActionResult> ProductsByCategory(string name, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            localProducts = await getLocalProductsByCategory(id);
            undercuttersProducts = await undercuttersProductGetter.GetProductsByCategoryName(name);
            mergeProducts();
            return View(mergedProducts);
        }

        public async Task<IActionResult> ProductsByBrand(string name, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            localProducts = await getLocalProductsByBrand(id);
            undercuttersProducts = await undercuttersProductGetter.GetProductsByBrandName(name);
            mergeProducts();
            return View(mergedProducts);
        }

        public Producks.Data.Category generateCategory(CategoryVM category)
        {
            return new Producks.Data.Category
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
            };
        }

        private void setupUndercutersClient()
        {
            undercuttersClient = new HttpClient();
            undercuttersClient.BaseAddress = new Uri(_configuration["UndercuttersBaseUri"]);
            undercuttersClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            undercuttersClient.Timeout = TimeSpan.FromSeconds(5);
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
                });
            }
            mergedProducts.OrderBy(b => b.Name);
        }

        private async Task<IList<ProductVM>> getLocalProducts()
        {
            return _mapper.Map<IList<ProductModel>, IList<ProductVM>>(
                await _productRepository.GetProducts());
        }

        private async Task<IList<ProductVM>> getLocalProductsByBrand(int? id)
        {
            return _mapper.Map<IList<ProductModel>, IList<ProductVM>>(
                await _productRepository.GetProductsByBrand(id));
        }

        private async Task<IList<ProductVM>> getLocalProductsByCategory(int? id)
        {
            return _mapper.Map<IList<ProductModel>, IList<ProductVM>>(
                await _productRepository.GetProductsByCategory(id));
        }
    }
}