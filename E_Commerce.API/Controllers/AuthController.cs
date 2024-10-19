using E_Commerce.Domain.Enums;
using E_Commerce.Service.DTOs;
using E_Commerce.Service.DTOs.User;
using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {
            if (userLogin == null)
                return BadRequest("User data is required");
            if (string.IsNullOrEmpty(userLogin.Email) || string.IsNullOrEmpty(userLogin.Password))
                return BadRequest("Please provide username and password");

            var user = await _userService.GetUser(x => x.Email == userLogin.Email && x.Password == userLogin.Password);
            if (user == null)
                return BadRequest("Invalid username or password");

            if (user.Role.ToString() != "ProductOwner" && user.Role.ToString() != "SuperAdmin")
                return StatusCode(StatusCodes.Status403Forbidden, "Access is restricted based on your role.");

            // Generate access token
            var token = GenerateJwtToken(user.FirstName, user.Role.ToString());

            // Generate refresh token
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // Set expiry for the refresh token (e.g., 7 days)

            var userUpdateDto = new UserUpdateDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                City = user.City,
                Password = user.Password,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiry
            };
            await _userService.UpdateUserAsync(userUpdateDto); 

            return Ok(new { Token = token, RefreshToken = refreshToken });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshDto tokenRefreshDto)
        {
            if (tokenRefreshDto == null || string.IsNullOrEmpty(tokenRefreshDto.RefreshToken))
                return BadRequest("Refresh token is required");

            // Get user by refresh token
            var user = await _userService.GetUserByRefreshToken(tokenRefreshDto.RefreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            // Generate new access token
            var newToken = GenerateJwtToken(user.FirstName, user.Role.ToString());

            // Optionally, you can generate a new refresh token
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(2); // Set new expiry for the refresh token


            var userUpdateDto = new UserUpdateDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                City = user.City,
                Password = user.Password,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiry
            };
            await _userService.UpdateUserAsync(userUpdateDto); // Save the user with the new refresh token and expiry

            return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private string GenerateJwtToken(string username, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = jwtSettings.GetValue<string>("Key");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role) 
            };

            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("ExpireMinutes"));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
