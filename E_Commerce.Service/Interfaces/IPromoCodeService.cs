using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.PromoCode;
namespace E_Commerce.Service.Interfaces
{
    public interface IPromoCodeService
    {
        Task<IEnumerable<PromoCode>> GetAllCodes();
        Task<IEnumerable<PromoCode>> GetActivePromoCodes();
        Task<IEnumerable<PromoCode>> GetInActivePromoCodes();
        Task<PromoCode> GetPromoCode(string code);
        Task<PromoCode> GetPromoCodeById(long id);
        Task<PromoCode> CreatePromoCode(PromoCreateDto promoCreate);
        Task<PromoCode> UpdatePromoCode(PromoUpdateDto promoUpdate);
        Task<bool> DeletePromoCode(long id);

        
    }
}
