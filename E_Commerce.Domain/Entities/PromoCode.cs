using System;
using E_Commerce.Domain.Commons;

namespace E_Commerce.Domain.Entities
{
    public class PromoCode : Auditable
    {
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ExpireDate { get; set; }
        public string OwnerId { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
