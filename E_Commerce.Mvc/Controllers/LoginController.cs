using E_Commerce.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace E_Commerce.Mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("LoginClient");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var loginData = new
                {
                    Email = model.Email,
                    Password = model.Password
                };

                var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7035/") };
                var loginResponse = await httpClient.PostAsJsonAsync("api/auth/login", loginData);

                if (loginResponse.IsSuccessStatusCode)
                {

                    var loginResultJson = await loginResponse.Content.ReadAsStringAsync();
                    using var jsonDoc = JsonDocument.Parse(loginResultJson);

                    // Extract the values manually
                    string accessToken = jsonDoc.RootElement.GetProperty("token").GetString();
                    string refreshToken = jsonDoc.RootElement.GetProperty("refreshToken").GetString();


                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        // Store the access token in session or a secure cookie
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true, // Prevents access to the cookie from JavaScript
                            Secure = true,   // Ensures the cookie is sent only over HTTPS
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30) // Set cookie expiration
                        };

                        Response.Cookies.Append("AccessToken", accessToken, cookieOptions);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to retrieve the access token.");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Email or password is incorrect.";
                }
            }

            return View(model);
        }

        //[HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AccessToken");
            var token = Request.Cookies["AccessToken"];
            return RedirectToAction("Index", "Home");
        }


    }
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

}
