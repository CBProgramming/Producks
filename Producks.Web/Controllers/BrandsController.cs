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
    public class BrandsController : Controller
    {
        private readonly StoreDb _context;
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public BrandsController(StoreDb context, IBrandRepository brandRepository, IMapper mapper)
        {
            _context = context;
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        // GET: Brands
        public async Task<IActionResult> Index()
        {
            var brands = _mapper.Map<IEnumerable<BrandVM>>(await _brandRepository.GetBrands());
            return View(brands);
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            BrandVM brandVM = _mapper.Map<BrandVM>(await _brandRepository.GetBrand(id));
            if (brandVM == null)
            {
                return NotFound();
            }

            return View(brandVM);
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] BrandVM brand)
        {
            if (ModelState.IsValid 
                && brand.Name != null 
                && !brand.Name.Equals("") 
                && await _brandRepository.CreateBrand(_mapper.Map<BrandModel>(brand)))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BrandVM brand = _mapper.Map<BrandVM>(_brandRepository.GetBrand(id));
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Active")] BrandVM brand)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid && brand.Name != null)
            {
                try
                {
                    _context.Update(generateBrand(brand));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BrandVM brand = _mapper.Map<BrandVM>(_brandRepository.GetBrand(id));
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            brand.Active = false;
            try
            {
                _context.Update(brand);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(brand.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(b => b.Id == id);
        }

        public Brand generateBrand(BrandVM brand)
        {
            return new Brand
            {
                Id = brand.Id,
                Name = brand.Name,
                Active = true
            };
        }
    }
}
