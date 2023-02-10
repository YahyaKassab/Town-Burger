using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Dto;

namespace Town_Burger.Models.Identity
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        public IEnumerable<Order>? Orders { get;set; }
        public IEnumerable<Address>? Addresses { get; set; }
        public IEnumerable<Deposit>? DepositsCustomer { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
