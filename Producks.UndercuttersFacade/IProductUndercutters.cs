using Producks.UndercuttersFacade.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Producks.UndercuttersFacade
{
    public interface IProductUndercutters
    {
        Task<List<ProductDtoUndercutters>> GetProducts();
        Task<List<ProductDtoUndercutters>> GetProductsByBrandName(string brandName);
        Task<List<ProductDtoUndercutters>> GetProductsByCategoryName(string categoryName);
    }
}
