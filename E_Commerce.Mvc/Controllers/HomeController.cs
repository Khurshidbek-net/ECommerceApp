using E_Commerce.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace E_Commerce.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ProductClient");
        }

        public async Task<IActionResult> Index()
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            var response = await _httpClient.GetAsync("api/products/get-all");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<IEnumerable<Product>>(content, jsonOptions);

            return View(products);
        }
    }

}
