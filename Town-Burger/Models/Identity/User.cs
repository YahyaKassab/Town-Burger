using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models.Identity
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        //Customer
        public Cart? Cart{ get; set; }
        public IEnumerable<Order>? Orders { get;set; }
        public IEnumerable<Address>? Addresses { get; set; }
        public IEnumerable<Deposit<User>>? DepositsCustomer { get; set; }

        //Employee
        public Int64? Salary { get; set; }
        public DateTime? ContractBegins { get; set; }
        public DateTime? ContractEnds { get; set; }
        public string? DaysOfWork { get; set; }
        public IEnumerable<Spend<User>>? SpendsEmployee { get; set; }
    }
}
