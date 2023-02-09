using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models.Dto
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
    public class LoginDtoPhone
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
