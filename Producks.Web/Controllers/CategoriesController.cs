using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Producks.Data;
using Producks.Web.Models;
using ProducksRepository;
using ProducksRepository.Models;

namespace Producks.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly StoreDb _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(StoreDb context, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = _mapper.Map<List<CategoryVM>>(_categoryRepository.GetCategories());
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CategoryVM category = _mapper.Map<CategoryVM>(_categoryRepository.GetCategory(id));
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Active")] CategoryVM category)
        {
            if (ModelState.IsValid
                && !String.IsNullOrEmpty(category.Name)
                && !String.IsNullOrEmpty(category.Description)
                && await _categoryRepository.CreateCategory(_mapper.Map<CategoryModel>(category)))
            {
                return base.RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CategoryVM category = _mapper.Map<CategoryVM>(_categoryRepository.GetCategory(id));
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Active")] CategoryVM category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid
                && !String.IsNullOrEmpty(category.Name)
                && !String.IsNullOrEmpty(category.Description)
                && await _categoryRepository.EditCategory(_mapper.Map<CategoryModel>(category)))
            {

                return base.RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            CategoryVM category = _mapper.Map<CategoryVM>(_categoryRepository.GetCategory(id));
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _categoryRepository.DetelteCategory(id))
            {
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }


        public Category generateCategory(CategoryVM category)
        {
            return new Category
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Active = true
            };
        }
    }
}
