using E_Commerce.Domain.Commons;
using E_Commerce.Domain.Enums;

namespace E_Commerce.Domain.Entities;

public class Product : Auditable
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public Category Category { get; set; }
}
