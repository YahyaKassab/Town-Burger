using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Town_Burger.Models.Identity;

namespace Town_Burger.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string Details { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<Order> Orders { get;set; }
    }
}
