using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUser(Expression<Func<User, bool>> expression, string[] includes = null, bool isTracking = true);
        Task<UserCreateDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task<UserUpdateDto> UpdateUserAsync(UserUpdateDto userUpdateDto);
        Task<User> GetUserByRefreshToken(string refreshToken);
        Task<bool> DeleteUserAsync(long id);
        Task<bool> UserExistsAsync(string email);
        Task UpdateUserRefreshTokenAsync(long id, string refreshToken, DateTime refreshTokenExpiry);
    }
}
