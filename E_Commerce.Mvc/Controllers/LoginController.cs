using E_Commerce.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace E_Commerce.Mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("LoginClient");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            var loginJson = JsonSerializer.Serialize(loginModel);
            var content = new StringContent(loginJson, Encoding.UTF8, "application/json");

            try
            {
                // Send login request to the API
                var response = await _httpClient.PostAsync("api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the response if a token or other info is returned
                    var token = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent)?["token"];

                    // Save the token in session or cookie if necessary
                    HttpContext.Session.SetString("AuthToken", token);

                    // Redirect to Home page on successful login
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
                    return View(loginModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again.");
                Console.WriteLine($"Exception during login: {ex.Message}");
                return View(loginModel);
            }
        }

    }
}
