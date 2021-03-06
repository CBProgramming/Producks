﻿using ProducksRepository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProducksRepository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryModel>> GetCategories();
        Task<CategoryModel> GetCategory(int? id);
        Task<bool> CreateCategory(CategoryModel category);
        Task<bool> EditCategory(CategoryModel category);
        Task<bool> DetelteCategory(int? id);
    }
}
