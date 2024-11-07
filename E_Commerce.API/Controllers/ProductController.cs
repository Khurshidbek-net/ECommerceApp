using E_Commerce.Data.Repositories;
using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.Product;
using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace E_Commerce.API.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IGenericRepository<Product> _genericRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IProductService productService, IWebHostEnvironment webHostEnvironment, IGenericRepository<Product> genericRepository)
    {
        _productService = productService;
        _webHostEnvironment = webHostEnvironment;
        _genericRepository = genericRepository;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
        => Ok(await _productService.GetAllProductsAsync());

    [HttpGet("get-my-products")]
    public async Task<IActionResult> GetAllAsync(string OwnerId)
    {
        var products = await _productService.GetAllProductsAsync();
        var filteredProducts = products.Where(p => p.OwnerId == OwnerId).ToList();
        return Ok(filteredProducts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(long id) => Ok(await _productService.GetProductByIdAsync(id));

    [Authorize(Roles = "ProductOwner")]
    [HttpPost("add")]
    public async Task<IActionResult> AddAsync([FromForm] ProductCreateDto product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Handle image upload
        if (product.ImageUrl != null && product.ImageUrl.Length > 0)
        {
            var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolderPath);

            // Generate a unique file name
            var filePath = Path.Combine(uploadsFolderPath, $"{Guid.NewGuid()}_{product.ImageUrl.FileName}");

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageUrl.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                // Log the error (you can use a logging framework)
                Console.WriteLine($"File upload error: {ex.Message}");
                return StatusCode(500, "Error saving the image file.");
            }
        }

        var access = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(access))
        {
            return Forbid("Access token is missing.");
        }

        var handler = new JwtSecurityTokenHandler();
        string userRoleClaim;
        string userIdClaim;

        try
        {
            var token = handler.ReadJwtToken(access);
            userRoleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            userIdClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("Owner ID not found in token.");
            }

            if (userRoleClaim != "ProductOwner")
            {
                return Forbid("You are not authorized to perform this action.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token error: {ex.Message}");
            return StatusCode(500, "Error processing the access token.");
        }

        // Create a new product object
        var newProduct = new Product
        {
            OwnerId = userIdClaim
        };

        try
        {
            var createdProduct = await _productService.CreateProductAsync(product);
            return Ok(createdProduct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating product: {ex.Message}");
            return StatusCode(500, "Error creating the product.");
        }
    }



    [Authorize(Roles = "SuperAdmin, ProductOwner")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync([FromForm] ProductUpdateDto product)
        => Ok(await _productService.UpdateProductAsync(product));


    [HttpPost("apply-promo")]
    public async Task<IActionResult> ApplyPromoCodesToSelectedProducts([FromBody] ApplyPromoCodesRequest request)
    {

        var products = _genericRepository.GetAll(x => request.Products.Contains(x.Id));
        var result = await _productService.ApplyPromoCodes(products, request.PromoId);
        if (result)
        {
            return Ok("Promo code applied successfully.");
        }

        return BadRequest("Failed to apply promo code.");


    }
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
        => Ok(await _productService.DeleteProductAsync(id));
}


