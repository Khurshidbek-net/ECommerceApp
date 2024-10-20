﻿using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.User;
using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers() => 
            Ok(await _userService.GetAllUsersAsync());

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] UserCreateDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _userService.CreateUserAsync(user));
        }
            

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDto user) =>
            Ok(await _userService.UpdateUserAsync(user));

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] long id) =>
            Ok(await _userService.DeleteUserAsync(id));

        [HttpGet("check-user-exists")]
        public async Task<IActionResult> CheckUserExists(string email)
        {
            var userExists = await _userService.UserExistsAsync(email); // Replace with actual check
            return Ok(userExists); // Return true or false
        }

    }
}
