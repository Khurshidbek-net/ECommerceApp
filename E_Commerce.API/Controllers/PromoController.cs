using AutoMapper;
using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.PromoCode;
using E_Commerce.Service.Interfaces;
using E_Commerce.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        {
            var promos = await _promoService.GetAllCodes();

            foreach (var promo in promos)
            {
                promo.IsActive = promo.ExpireDate >= DateTime.Now;
                var updatePromo = _map.Map<PromoUpdateDto>(promo);
                updatePromo.ExpireDate = promo.ExpireDate;
                await _promoService.UpdatePromoCode(updatePromo);
            }

            return Ok(promos);
        }

        [HttpGet("active")]
        [Authorize(Roles = "ProductOwner")]
        public async Task<ActionResult<IEnumerable<PromoCode>>> GetActivePromos()
            => Ok(await _promoService.GetActivePromoCodes());

        [Authorize(Roles = "ProductOwner")]
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<PromoCode>>> GetInactivePromos()
            => Ok(await _promoService.GetInActivePromoCodes());


        [HttpGet("get-by-id/{id}")]
        public async Task<ActionResult<PromoCode>> GetPromo(long id)
        {
            var promo = await _promoService.GetPromoCodeById(id);
            var updatePromo = _map.Map<PromoUpdateDto>(promo);

            if (promo == null)
            {
                return NotFound("Promo code not found.");
            }

            
            

            return Ok(promo);
        }

        [HttpGet("get-my-promos/{id}")]
        public async Task<IActionResult> GetPromos(long id)
        {
            var promos = await _promoService.GetAllCodes();
            var myPromos = promos.Where(x => x.OwnerId == id.ToString());
            if(myPromos == null)
                return NotFound("Promo code not found.");

            foreach (var promo in myPromos)
            {
                promo.IsActive = promo.ExpireDate >= DateTime.Now;
                var updatePromo = _map.Map<PromoUpdateDto>(promo);
                updatePromo.ExpireDate = promo.ExpireDate;
                await _promoService.UpdatePromoCode(updatePromo);
            }
            return Ok(myPromos);
        }


        [Authorize(Roles = "ProductOwner")]
        [HttpPost("create-promo")]
        public async Task<ActionResult<PromoCode>> CreatePromo([FromForm] PromoCreateDto promoCode)
        {
            var access = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            string userRoleClaim;
            string userIdClaim;

            try
            {
                var token = handler.ReadJwtToken(access);
                userRoleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                userIdClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return BadRequest("Owner ID not found in token.");
                }

                if (userRoleClaim != "ProductOwner")
                {
                    return Forbid("You are not authorized to perform this action.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token error: {ex.Message}");
                return StatusCode(500, "Error processing the access token.");
            }

            // Create a new product object
            var newPromo = new PromoCode
            {
                OwnerId = userIdClaim
            };

            try
            {
                var createdPromo = await _promoService.CreatePromoCode(promoCode);
                return Ok(createdPromo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
                return StatusCode(500, "Error creating the product.");
            }

        }

        //[Authorize(Roles = "ProductOwner")]
        [HttpPut("update-promo")]
        public async Task<ActionResult<PromoCode>> UpdatePromo(long id, [FromBody] PromoUpdateDto promoUpdateDto)
        {
            if (promoUpdateDto == null)
            {
                return BadRequest("Promo update data is required.");
            }

            if(id != promoUpdateDto.Id)
            {
                return BadRequest("Invalid promo code ID provided.");
            }

            // Fetch all promo codes and find the specific promo code by the provided code
            var promo = await _promoService.GetPromoCodeById(id);
            //GetPromo();

            if (promo == null)
            {
                return NotFound("Promo code not found.");
            }


            // Update the promo code in the database
            var updatedPromo = await _promoService.UpdatePromoCode(promoUpdateDto);

            return Ok(updatedPromo);
        }

        //[Authorize(Roles = "ProductOwner")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePromo(string code)
        {
            var codes = _promoService.GetAllCodes();
            var promo = codes.Result.FirstOrDefault(x => x.Code == code);
            if (promo is null)
                return NotFound("Promo code not found");
            await _promoService.DeletePromoCode(promo.Id);
            return Ok("Promo code deleted");
        }

    }
}
