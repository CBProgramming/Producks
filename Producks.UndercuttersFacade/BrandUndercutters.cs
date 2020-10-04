using Microsoft.Extensions.Configuration;
using Producks.UndercuttersFacade.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Producks.UndercuttersFacade
{
    public class BrandUndercutters : IBrandUndercutters
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public BrandUndercutters(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient()
            {
                BaseAddress = new Uri(_configuration["UndercuttersBaseUri"]),
                Timeout = TimeSpan.FromSeconds(5)
            };
            _client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        public async Task<List<BrandDtoUndercutters>> GetBrands()
        {
            List<BrandDtoUndercutters> brands;
            string uri = "/api/Brand";
            try
            {
                var response = await _client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                brands = await response.Content.ReadAsAsync<List<BrandDtoUndercutters>>();
            }
            catch (HttpRequestException e)
            {
                brands = new List<BrandDtoUndercutters>();
            }
            return brands;
        }
    }
}
