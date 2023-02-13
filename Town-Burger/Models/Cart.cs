using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Identity;

namespace Town_Burger.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public ICollection<CartItem>? Items { get; set; }
        public double TotalPrice { get; set; } = 0;
        public int? OrderId { get; set; }

        public Order? Order { get; set; }
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }


    }
}
