using E_Commerce.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using E_Commerce.Mvc.Enums;

namespace E_Commerce.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AccountClient");
        }

        public IActionResult Register()
        {
            var roles = Enum.GetValues(typeof(UserRole))
                    .Cast<UserRole>()
                    .Select(r => new SelectListItem
                    {
                        Value = r.ToString(),
                        Text = r.ToString()
                    }).ToList();

            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the user already exists
                var userExistsResponse = await _httpClient.GetAsync($"api/User/check-user-exists?email={model.Email}");

                if (userExistsResponse.IsSuccessStatusCode)
                {
                    var exists = await userExistsResponse.Content.ReadAsStringAsync();
                    if (bool.TryParse(exists, out bool userExists) && userExists)
                    {
                        TempData["ErrorMessage"] = "User already exists. Please use a different email address.";
                        // Return roles again if the registration fails
                        var roless = GetRoles();
                        ViewBag.Roles = roless;
                        return View(model);
                    }
                }

                // Proceed to register the user
                var user = new
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    City = model.City,
                    Role = model.Role.ToString()
                };

                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/User/add-user", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Registration failed: {responseContent}");
                }
            }

            // Return roles if model state is invalid or user already exists
            var roles = GetRoles();
            ViewBag.Roles = roles;

            return View(model);
        }

        private List<SelectListItem> GetRoles()
        {
            return Enum.GetValues(typeof(UserRole))
                        .Cast<UserRole>()
                        .Select(r => new SelectListItem
                        {
                            Value = r.ToString(),
                            Text = r.ToString()
                        }).ToList();
        }

    }
}
