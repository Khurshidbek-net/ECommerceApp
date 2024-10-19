using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using E_Commerce.Mvc.Models;

namespace E_Commerce.Mvc.Controllers
{
    public class PromocodeController : Controller
    {
        private readonly HttpClient _httpClient;
        public PromocodeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PromoClient");
        }

        public async Task<IActionResult> Index()
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            var response = await _httpClient.GetAsync("api/Promo/get-all-promos");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var content = await response.Content.ReadAsStringAsync();
            var promos = JsonSerializer.Deserialize<IEnumerable<PromoCode>>(content, jsonOptions);

            return View(promos);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
