
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

    [HttpGet]
    [Route("category&product")]
    public async Task<IActionResult> GetCategoryAndProduct([FromQuery] Category category)
    {
        var number = await _service.GetNumberOfProduct(category);
        var all = await _service.GetAllProductsAsync();
        var totalPrice =  all.Where(x => x.Category == category).Sum(x => x.Price);

        TotalProduct totalProduct = new TotalProduct()
        {
            Category = category,
            NumberOfProduct = number,
            TotalPrice = totalPrice
        };

        return Ok(totalProduct);
    }
       


}


public class TotalProduct
{
    public Category Category { get; set; }
    public int NumberOfProduct { get; set; }
    public decimal TotalPrice { get; set; }
}

