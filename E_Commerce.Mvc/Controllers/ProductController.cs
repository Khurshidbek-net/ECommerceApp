using E_Commerce.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

public class ProductController : Controller
{
    private readonly HttpClient _httpClient;

    public ProductController(IHttpClientFactory httpClientFactory)
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

        foreach (var item in products)
        {
            Console.WriteLine(item.Name);
            Console.WriteLine(item.ImageUrl);
        }

        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            string imageUrl = null;

            // Step 1: Upload image
            if (model.ImageFile != null)
            {
                using var content = new MultipartFormDataContent();
                var fileStreamContent = new StreamContent(model.ImageFile.OpenReadStream())
                {
                    Headers = { ContentType = new MediaTypeHeaderValue(model.ImageFile.ContentType) }
                };
                content.Add(fileStreamContent, "file", model.ImageFile.FileName);

                var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7035/") };
                var uploadResponse = await httpClient.PostAsync("api/upload", content);

                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadFromJsonAsync<dynamic>();
                    imageUrl = uploadResult?.ImageUrl;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to upload the image.");
                    return View(model);
                }
            }

            // Step 2: Create the product
            var product = new
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = imageUrl,
                Category = model.Category
            };

            var createResponse = await _httpClient.PostAsJsonAsync("api/products", product);
            if (createResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to create the product.");
                return View(model);
            }
        }

        return View(model);
    }




}
