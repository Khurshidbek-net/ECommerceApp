using Microsoft.AspNetCore.Authorization;
using E_Commerce.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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

        return View(products);
    }

    public IActionResult Create()
    {   
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var accessToken = Request.Cookies["AccessToken"]; 

            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["ErrorMessage"] = "You are not authorized to perform this action.";
                //ModelState.AddModelError(string.Empty, "You are not authorized to perform this action.");
                return View(model);
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);

            var userRoleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (userRoleClaim != "ProductOwner")
            {
                TempData["ErrorMessage"] = "Only Product Owners are allowed to create products.";
                return RedirectToAction("Login", "Login");
            }

            //if (!hasRequiredRole)
            //{
            //    TempData["ErrorMessage"] = "Only Product Owners are allowed to create products.";
            //    return View(model);
            //}
            var content = new MultipartFormDataContent();
            
            if (model.ImageUrl != null)
            {
                var fileStreamContent = new StreamContent(model.ImageUrl.OpenReadStream())
                {
                    Headers = { ContentType = new MediaTypeHeaderValue(model.ImageUrl.ContentType) }
                };
                content.Add(new StringContent(model.Name), "Name");
                content.Add(new StringContent(model.Description), "Description");
                content.Add(new StringContent(model.Price.ToString()), "Price");
                content.Add(new StringContent(model.Category.ToString()), "Category");
                content.Add(new StringContent(model.CreatedAt.ToString()), "CreatedAt");
                content.Add(new StringContent(model.UpdatedAt.ToString()), "UpdatedAt");
                content.Add(fileStreamContent, "ImageUrl", model.ImageUrl.FileName); // Ensure property name matches DTO
            }

            // Add other product properties

            var createHttpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7035/") };
            createHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var createResponse = await createHttpClient.PostAsync("api/products/add", content); // Adjust route as needed
            if (createResponse.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product created successfully.";
                return View(model);
            }
            else
            {
                var errorMessage = await createResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Failed to create the product. Details: {errorMessage}");
                return View(model);
            }
        }

        return View(model);
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        var response = await _httpClient.DeleteAsync($"api/products/{id}");

        if (!response.IsSuccessStatusCode)
        {
            // If the delete request fails, return an error view or message
            return View("Error");
        }

        // Redirect back to the list of products after successful deletion
        return RedirectToAction("Index");
    }



}
