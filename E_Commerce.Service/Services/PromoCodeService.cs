using AutoMapper;
using E_Commerce.Data.Repositories;
using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.PromoCode;
using E_Commerce.Service.Exceptions;
using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace E_Commerce.Service.Services
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly IGenericRepository<PromoCode> _genericRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public PromoCodeService(IGenericRepository<PromoCode> genericRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<PromoCode>> GetAllCodes()
        {
            var codes = _genericRepository.GetAll(null!).AsNoTracking().ToListAsync();
            return await Task.FromResult(codes.Result);

        }

        public async Task<PromoCode> CreatePromoCode(PromoCreateDto promoCreate)
        {
            var expireDate = DateTime.Now.AddMinutes(promoCreate.ExpireAfterMinutes);
            var promoCode = _mapper.Map<PromoCode>(promoCreate);

            promoCode.ExpireDate = expireDate;

            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var userId = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            promoCode.OwnerId = userId;

            await _genericRepository.CreateAsync(promoCode);
            await _genericRepository.SaveChangesAsync();
            return promoCode;
        }

        public async Task<bool> DeletePromoCode(long id)
        {
            var promode = await _genericRepository.GetAsync(x => x.Id == id);
            if (promode == null)
                return false;
            await _genericRepository.DeleteAsync(id);
            await _genericRepository.SaveChangesAsync();
            return true;
        }

        public async Task<PromoCode> GetPromoCode(string code)
        {
            var promocode = await _genericRepository.GetAsync(x => x.Code == code);
            if(promocode == null)
                throw new CustomException("Promo code not found", 404);
            return promocode;
        }

        public async Task<PromoCode> GetPromoCodeById(long id)
        {
            var promocode = await _genericRepository.GetAsync(x => x.Id == id);
            if (promocode == null)
                throw new CustomException("Promo code not found", 404);
            return promocode;
        }

        public async Task<PromoCode> UpdatePromoCode(PromoUpdateDto promoUpdate)
        {
            var promoCode = await _genericRepository.GetAsync(x => x.Id == promoUpdate.Id);
            if(promoCode == null)
                throw new CustomException("Promo code not found", 404);


            promoCode.Code = promoUpdate.Code;
            promoCode.DiscountPercent = promoUpdate.DiscountPercent;

            promoCode.ExpireDate = promoUpdate.ExpireDate;
            _genericRepository.Update(promoCode);
            await _genericRepository.SaveChangesAsync();
            return promoCode;
        }

        public async Task<IEnumerable<PromoCode>> GetActivePromoCodes()
        {
            var activeCodes = await _genericRepository.GetAll(x => x.IsActive == true).AsNoTracking().ToListAsync();
            return activeCodes;
        }

        public async Task<IEnumerable<PromoCode>> GetInActivePromoCodes()
        {
            var activeCodes = await _genericRepository.GetAll(x => x.IsActive == false).AsNoTracking().ToListAsync();
            return activeCodes;
        }
    }
}
