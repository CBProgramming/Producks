using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Producks.UndercuttersFacade.Models;

namespace Producks.UndercuttersFacade
{
    public class ProductUndercutters : IProductUndercutters
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly IBrandUndercutters _brandGetter;

        public ProductUndercutters(IConfiguration configuration, IBrandUndercutters brandGetter)
        {
            _configuration = configuration;
            _brandGetter = brandGetter;
            _client = new HttpClient()
            {
                BaseAddress = new Uri(_configuration["UndercuttersBaseUri"]),
                Timeout = TimeSpan.FromSeconds(5)
            };
            _client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        public async Task<List<ProductDtoUndercutters>> GetProducts()
        {
            List<ProductDtoUndercutters> products;
            string uri = "/api/Product";
            try
            {
                var response = await _client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                products = await response.Content.ReadAsAsync<List<ProductDtoUndercutters>>();
            }
            catch (HttpRequestException e)
            {
                products = new List<ProductDtoUndercutters>();
            }
            return products;
        }

        public async Task<List<ProductDtoUndercutters>> GetProductsByBrandName(string brandName)
        {
            List<ProductDtoUndercutters> products;
            int categoryId = -1;
            List<BrandDtoUndercutters> undercuttersBrands = await _brandGetter.GetBrands();
            foreach (BrandDtoUndercutters brand in undercuttersBrands)
            {
                if (brand.Name.Equals(brandName))
                {
                    categoryId = brand.Id;
                }
            }
            if (categoryId != -1)
            {
                string uri = "/api/Product?brand_id=" + categoryId;
                try
                {
                    var response = await _client.GetAsync(uri);
                    response.EnsureSuccessStatusCode();
                    products = await response.Content.ReadAsAsync<List<ProductDtoUndercutters>>();
                }
                catch (HttpRequestException e)
                {
                    products = new List<ProductDtoUndercutters>();
                }
            }
            else
            {
                products = new List<ProductDtoUndercutters>();
            }
            return products;
        }

        public async Task<List<ProductDtoUndercutters>> GetProductsByCategoryName(string categoryName)
        {
            List<ProductDtoUndercutters> products;
            string uri = "/api/Product?category_name=" + categoryName;
            try
            {
                var response = await _client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                products = await response.Content.ReadAsAsync<List<ProductDtoUndercutters>>();
            }
            catch (HttpRequestException e)
            {
                products = new List<ProductDtoUndercutters>();
            }
            return products;
        }
    }
}