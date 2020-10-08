using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProducksRepository.Models
{
    public class StoreIndexModel
    {
        public List<CategoryModel> Categories { get; set; }
        public List<BrandModel> Brands { get; set; }
    }
}
