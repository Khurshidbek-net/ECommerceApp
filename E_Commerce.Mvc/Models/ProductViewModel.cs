using E_Commerce.Mvc.Commons;
using E_Commerce.Mvc.Enums;

namespace E_Commerce.Mvc.Models;
    public class ProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public IFormFile ImageUrl { get; set; }
        public Category Category { get; set; }
    }
