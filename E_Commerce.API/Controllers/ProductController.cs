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

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllAsync() => Ok(await _productService.GetAllProductsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(long id) => Ok(await _productService.GetProductByIdAsync(id));

    [Authorize(Roles = "SuperAdmin, ProductOwner")]
    [HttpPost("add")]
    public async Task<IActionResult> AddAsync([FromForm] ProductCreateDto product)
        => Ok(await _productService.CreateProductAsync(product));

    [Authorize(Roles = "SuperAdmin, ProductOwner")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync([FromForm] ProductUpdateDto product)
        => Ok(await _productService.UpdateProductAsync(product));

    [Authorize(Roles = "SuperAdmin, ProductOwner")]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
        => Ok(await _productService.DeleteProductAsync(id));

}
