using Producks.UndercuttersFacade.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Producks.UndercuttersFacade
{
    public interface ICategory
    {
        Task<List<CategoryDtoUndercutters>> GetCategories();
    }
}
