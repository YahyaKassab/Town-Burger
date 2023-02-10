using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Identity;

namespace Town_Burger.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public IEnumerable<CartItem>? Items { get; set; }
        [Required]
        public int? OrderId { get; set; }
        [Required]
        public Order? Order { get; set; }
        [Required]
        public int? CustomerId { get; set; }
        [Required]
        public Customer? Customer { get; set; }

    }
}
