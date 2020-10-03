using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Producks.Web.Models
{
    public class StoreIndexVM
    {

        public List<CategoryVM> Categories { get; set; }
        public List<BrandVM> Brands { get; set; }
    }
}
