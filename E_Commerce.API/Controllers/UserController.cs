using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.User;
using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        public UserController(IUserService userService, IProductService productService)
        {
            _userService = userService;
            _productService = productService;
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers() => 
            Ok(await _userService.GetAllUsersAsync());

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] UserCreateDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _userService.CreateUserAsync(user));
        }
            

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDto user) =>
            Ok(await _userService.UpdateUserAsync(user));

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] long id) =>
            Ok(await _userService.DeleteUserAsync(id));

        [HttpGet("check-user-exists")]
        public async Task<IActionResult> CheckUserExists(string email)
        {
            var userExists = await _userService.UserExistsAsync(email); // Replace with actual check
            return Ok(userExists); // Return true or false
        }

        [HttpGet("get-all-products-by-owner/{name}")]
        public async Task<IActionResult> GetAllProductByOwner(string name)
        {
            var owners = await _userService.GetAllUsersAsync();
            var owner = owners.FirstOrDefault(x => x.FirstName == name);
            if(owner == null)
            {
                return NotFound("Owner not found");
            };
            var ownerId = owner.Id;
            var products = await _productService.GetAllProductsAsync();
            var ownerProducts = products.Where(x => x.OwnerId == ownerId.ToString()).ToList();

            return ownerProducts.Count > 0 ? Ok(ownerProducts) : NotFound("Owner has no products");
        }


        [HttpGet("get-owners-products-total-price/{name}")]
        public async Task<IActionResult> GetOwnersProductsTotalPrice(string name)
        {
            var owners = await _userService.GetAllUsersAsync();
            var owner = owners.FirstOrDefault(x => x.FirstName == name);
            if( owner == null)
            {
                return NotFound("Owner not found");
            };
            var ownerId = owner.Id;
            var products = await _productService.GetAllProductsAsync();
            var ownerProducts = products.Where(x => x.OwnerId == ownerId.ToString()).ToList();

            return ownerProducts.Count > 0 ? Ok(ownerProducts.Sum(x => x.Price)) : NotFound("Owner has no products");
        }

        [HttpGet("get-all-productOwners")]
        public async Task<IActionResult> GetAllOwners()
        {
            var users = await _userService.GetAllUsersAsync();
            var owners = users.Where(x => x.Role.ToString() == "ProductOwner");
            return Ok(owners);
        }

        [HttpGet("get-all-sellers-number")]
        public async Task<IActionResult> GetAllSellers()
        {
            var users = await _userService.GetAllUsersAsync();
            var sellers = users.Where(x => x.Role.ToString() == "ProductOwner");
            return Ok(sellers.Count());
        }

        [HttpGet("get-all-total-amount-of-products")]
        public async Task<IActionResult> GetAllTotalAmountOfProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products.Sum(x => x.Price));
        }


    }
}
