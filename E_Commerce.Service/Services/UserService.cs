using AutoMapper;
using E_Commerce.Data.Contexts;
using E_Commerce.Data.Repositories;
using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.User;
using E_Commerce.Service.Exceptions;
using E_Commerce.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_Commerce.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UserService(IGenericRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserCreateDto>  CreateUserAsync(UserCreateDto userCreateDto)
        {
            var user = new User
            {
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                Email = userCreateDto.Email,
                Password = userCreateDto.Password, // Consider hashing the password
                PhoneNumber = userCreateDto.PhoneNumber,
                City = userCreateDto.City,
                Role = userCreateDto.Role,
                RefreshToken = Guid.NewGuid().ToString(), // Generate a new token
                RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(10) // Set token expiry, for example, 30 days
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();
            return _mapper.Map<UserCreateDto>(user);

        }

        public async Task<bool> DeleteUserAsync(long id)
        {
            var user = await _userRepository.GetAsync(x => x.Id == id);
            if (user is null)
                return false;
            await _userRepository.DeleteAsync(id);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = _userRepository.GetAll(null!);
            return await Task.FromResult(users);

        }

        public async Task<User> GetUser(Expression<Func<User, bool>> expression, string[] includes = null, bool isTracking = true)
        {
            IQueryable<User> query = _userRepository.GetAll(null!);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (!isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(expression);
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            var user = await _userRepository.GetAll(null!).
                FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenExpiry > DateTime.UtcNow);
            return user;
        }

        public async Task<UserUpdateDto> UpdateUserAsync(UserUpdateDto userUpdateDto)
        {
            var user = _mapper.Map<User>(userUpdateDto);
            if (user == null)
                throw new CustomException("User not found", 404);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            return userUpdateDto;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            // Check if a user with the given email exists
            return await _userRepository.GetAll(null!)
                .AnyAsync(u => u.Email == email);
        }
    }
}
