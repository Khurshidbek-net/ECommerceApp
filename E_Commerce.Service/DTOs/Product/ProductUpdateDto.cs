using E_Commerce.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Service.DTOs.Product;

public class ProductUpdateDto
{
    public long Id { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public IFormFile ImageUrl { get; set; }
    public Category Category { get; set; }
}
