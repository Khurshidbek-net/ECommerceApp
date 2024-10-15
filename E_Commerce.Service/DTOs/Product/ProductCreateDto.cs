using E_Commerce.Domain.Enums;
using Microsoft.AspNetCore.Http;


namespace E_Commerce.Service.DTOs.Product;

public class ProductCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public IFormFile ImageUrl { get; set; }
    public Category Category { get; set; }
}
