using E_Commerce.Domain.Enums;
namespace E_Commerce.Service.DTOs.User;

public class UserCreateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string City { get; set; }
    public UserRole Role { get; set; }
}
