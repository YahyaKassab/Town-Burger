using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Identity;

namespace Town_Burger.Models.Dto
{
    public class RegisterEmployeeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public Int64 Salary { get; set; }
        [Required]
        public DateTime ContractBegins { get; set; }
        [Required]
        public DateTime ContractEnds { get; set; }
        public string? DaysOfWork { get; set; } = null;
        public IEnumerable<Spend<User>>? SpendsEmployee { get; set; } = null;

        public byte[]? Picture { get; set; } = null;
    }
}
