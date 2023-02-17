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
        public Cart Cart { get; set; }
        public ICollection<Order> Orders { get;set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Deposit> DepositsCustomer { get; set; }
        public ICollection<Review> Reviews{ get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
