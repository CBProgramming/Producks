using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Producks.Web.Models;
using ProducksRepository;
using ProducksRepository.Models;

namespace Producks.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, IBrandRepository brandRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IList<ProductModel>, IList<ProductVM>>(await _productRepository.GetProducts()));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM product = _mapper.Map<ProductVM>(await _productRepository.GetProduct(id));
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewData["BrandId"] = new SelectList(await _brandRepository.GetBrands(), "Id", "Name");
            ViewData["CategoryId"] = new SelectList(await _categoryRepository.GetCategories(), "Id", "Description");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,BrandId,Name,Description,Price,StockLevel,Active")] ProductVM product)
        {
            if (ModelState.IsValid
                && !String.IsNullOrEmpty(product.Name)
                && await _productRepository.CreateProduct(_mapper.Map<ProductModel>(product)))
            {
                return base.RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(await _brandRepository.GetBrands(), "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(await _categoryRepository.GetCategories(), "Id", "Description", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM product = _mapper.Map<ProductVM>(await _productRepository.GetProduct(id));
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(await _brandRepository.GetBrands(), "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(await _categoryRepository.GetCategories(), "Id", "Description", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,BrandId,Name,Description,Price,StockLevel,Active")] ProductVM product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid
                && !String.IsNullOrEmpty(product.Name)
                && await _productRepository.EditProduct(_mapper.Map<ProductModel>(product)))
            {

                return base.RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(await _brandRepository.GetBrands(), "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(await _categoryRepository.GetCategories(), "Id", "Description", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductVM product = _mapper.Map<ProductVM>(await _productRepository.GetProduct(id));
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _productRepository.DetelteProduct(id))
            {
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
