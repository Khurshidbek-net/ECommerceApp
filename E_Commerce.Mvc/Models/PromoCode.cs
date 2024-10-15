using E_Commerce.Mvc.Commons;

namespace E_Commerce.Mvc.Models
{
    public class PromoCode : Auditable
    {
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ExpireDate { get; set; }
    }
}
