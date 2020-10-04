using Producks.UndercuttersFacade.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Producks.UndercuttersFacade
{
    public interface IBrandUndercutters
    {
        Task<List<BrandDtoUndercutters>> GetBrands();
    }
}
