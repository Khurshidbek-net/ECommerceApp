using E_Commerce.Mvc.Enums;
using E_Commerce.Mvc.Models;

namespace E_Commerce.Mvc.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public Category Category { get; set; }
    }
}
