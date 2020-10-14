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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreDb _context;
        private readonly IMapper _mapper;

        public CategoryRepository(StoreDb context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CreateCategory(CategoryModel category)
        {
            try
            {
                _context.Add(_mapper.Map<Category>(category));
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> DetelteCategory(int? id)
        {
            var category = await _context.Categories.FindAsync(id);
            category.Active = false;
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> EditCategory(CategoryModel category)
        {
            try
            {
                _context.Update(_mapper.Map<Category>(category));
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public List<CategoryModel> GetCategories()
        {
            var categories = _context.Categories
                           .Where(b => b.Active == true)
                           .AsEnumerable()
                           .Select(b => _mapper.Map<Category, CategoryModel>(b))
                           .ToList();
            return categories;
        }

        public CategoryModel GetCategory(int? id)
        {
            return _mapper.Map<Category, CategoryModel>(
                _context.Categories
                .Where(c => c.Active == true)
                .FirstOrDefaultAsync(b => b.Id == id)
                .Result);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }
    }
}
