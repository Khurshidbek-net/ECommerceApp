using AutoMapper;
using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.PromoCode;
using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoController : ControllerBase
    {
        private readonly IPromoCodeService _promoService;
        private readonly IMapper _map;

        public PromoController(IPromoCodeService promoService, IMapper map)
        {
            _promoService = promoService;
            _map = map;
        }

        [HttpGet("get-all-promos")]
        public async Task<ActionResult<IEnumerable<PromoCode>>> GetAllPromos() 
            => Ok(await _promoService.GetAllCodes());

        [HttpGet("active")]
        [Authorize(Roles = "ProductOwner")]
        public async Task<ActionResult<IEnumerable<PromoCode>>> GetActivePromos()
            => Ok(await _promoService.GetActivePromoCodes());

        [Authorize(Roles = "ProductOwner")]
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<PromoCode>>> GetInactivePromos()
            => Ok(await _promoService.GetInActivePromoCodes());


        [HttpGet("get-by-code/{code}")]
        public async Task<ActionResult<PromoCode>> GetPromo(long code)
        {
            var promo = await _promoService.GetPromoCode(code);
            var updatePromo = _map.Map<PromoUpdateDto>(promo);

            if (promo == null)
            {
                return NotFound("Promo code not found.");
            }

            if (promo.ExpireDate <= DateTime.UtcNow)
            {
                promo.IsActive = false;
                await _promoService.UpdatePromoCode(updatePromo);
            }

            return Ok(promo);
        }


        [Authorize(Roles = "ProductOwner")]
        [HttpPost("create-promo")]
        public async Task<ActionResult<PromoCode>> CreatePromo([FromForm] PromoCreateDto promoCode)
            => Ok(await _promoService.CreatePromoCode(promoCode));

        [Authorize(Roles = "ProductOwner")]
        [HttpPut("update-promo")]
        public async Task<ActionResult<PromoCode>> UpdatePromo([FromForm] PromoUpdateDto promoCode)
        {
            if(promoCode == null)
            {
                return NotFound("Promo code not found.");
            }
            return Ok(await _promoService.UpdatePromoCode(promoCode));
        }

        [Authorize(Roles = "ProductOwner")]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeletePromo(long id)
        {
            var promo = await _promoService.GetPromoCode(id);
            if (promo is null)
                return NotFound("Promo code not found");
            await _promoService.DeletePromoCode(id);
            return Ok("Promo code deleted");
        }

    }
}
