
using E_Commerce.Domain.Enums;
using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers;

[Route("api/category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IProductService _service;

    public CategoryController(IProductService productService) => _service = productService;


    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery]Category category) => 
        Ok(await _service.GetProductsByCategoryAsync(category));

}

