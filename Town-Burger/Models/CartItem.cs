using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        [Required]
        public MenuItem Item { get; set; }

        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }
        [Required]
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
