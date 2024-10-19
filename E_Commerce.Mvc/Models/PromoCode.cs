using System;
using E_Commerce.Mvc.Commons;

namespace E_Commerce.Mvc.Models
{
    public class PromoCode
    {
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
        public int ExpireAfterMinutes { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
