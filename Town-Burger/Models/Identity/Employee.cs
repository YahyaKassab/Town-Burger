using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models.Identity
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        [Required]
        public Int64 Salary { get; set; }
        public DateTime? ContractBegins { get; set; }
        public DateTime? ContractEnds { get; set; }
        public string? DaysOfWork { get; set; }
        public IEnumerable<Spend>? SpendsEmployee { get; set; }
        public string? PictureSource { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public User User { get; set; }
    }
}
