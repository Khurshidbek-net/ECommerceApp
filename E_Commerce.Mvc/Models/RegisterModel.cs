using E_Commerce.Mvc.Commons;
using E_Commerce.Mvc.Enums;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Mvc.Models
{
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public UserRole Role { get; set; }
    }
}
