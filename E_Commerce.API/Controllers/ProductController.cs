using E_Commerce.Service.DTOs.Product;
using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IProductService productService, IWebHostEnvironment webHostEnvironment)
    {
        _productService = productService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllAsync() => Ok(await _productService.GetAllProductsAsync());

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
        else if (product.ImageUrl != null && product.ImageUrl.Length > 0)
        {

            var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolderPath);

            var filePath = Path.Combine(uploadsFolderPath, product.ImageUrl.FileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await product.ImageUrl.CopyToAsync(fileStream);
            }
        }

        var createdProduct = await _productService.CreateProductAsync(product);
        return Ok(createdProduct);
    }

    [Authorize(Roles = "SuperAdmin, ProductOwner")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync([FromForm] ProductUpdateDto product)
        => Ok(await _productService.UpdateProductAsync(product));

    //[Authorize(Roles = "ProductOwner")]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
        => Ok(await _productService.DeleteProductAsync(id));

}
