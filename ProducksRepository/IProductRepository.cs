using ProducksRepository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProducksRepository
{
    public interface IProductRepository
    {
        Task<List<ProductModel>> GetProducts();
        Task<List<ProductModel>> GetProductsByBrand(int? id);
        Task<List<ProductModel>> GetProductsByCategory(int? id);
        Task<ProductModel> GetProduct(int? id);
        Task<bool> CreateProduct(ProductModel product);
        Task<bool> EditProduct(ProductModel product);
        Task<bool> DetelteProduct(int? id);
    }
}
