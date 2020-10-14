using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Producks.Data;
using ProducksRepository.Models;

namespace ProducksRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreDb _context;

        public CategoryRepository(StoreDb context)
        {
            _context = context;
        }

        public Task<bool> CreateCategory(CategoryModel category)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DetelteCategory(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditCategory(CategoryModel category)
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryModel>> GetCategories()
        {
            throw new NotImplementedException();
        }

        public Task<CategoryModel> GetCategory(int? id)
        {
            throw new NotImplementedException();
        }
    }
}
