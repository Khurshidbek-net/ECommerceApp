using E_Commerce.Mvc.Enums;

namespace E_Commerce.Mvc.Models;
    public class ProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile ImageFile { get; set; } 
        public Category Category { get; set; }
    }
