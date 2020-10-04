using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Producks.UndercuttersFacade.Models;

namespace Producks.UndercuttersFacade
{
    public class Category : ICategory
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public Category(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient()
            {
                BaseAddress = new Uri(_configuration["UndercuttersBaseUri"]),
                Timeout = TimeSpan.FromSeconds(5)
            };
            _client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        public async Task<List<CategoryDtoUndercutters>> GetCategories()
        {
            List<CategoryDtoUndercutters> categories;
            string uri = "/api/Category";
            try
            {
                var response = await _client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                categories = await response.Content.ReadAsAsync<List<CategoryDtoUndercutters>>();
            }
            catch (HttpRequestException e)
            {
                categories = new List<CategoryDtoUndercutters>();
            }
            return categories;
        }
    }
}
