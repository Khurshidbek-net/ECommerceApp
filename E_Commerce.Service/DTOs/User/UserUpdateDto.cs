namespace E_Commerce.Service.DTOs.User
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
    }
}
