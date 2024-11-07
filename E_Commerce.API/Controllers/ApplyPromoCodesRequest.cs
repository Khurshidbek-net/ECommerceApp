using E_Commerce.Domain.Entities;

namespace E_Commerce.API.Controllers
{
    public class ApplyPromoCodesRequest
    {
        public IEnumerable<long> Products { get; set; }
        public long PromoId { get; set; }
    }
}