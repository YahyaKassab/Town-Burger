using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models.Responses
{
    public class CartItem
    {
        public int Id { get; set; }
        [Required]
        public MenuItem Item{ get;set; }

        public string? Description{ get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }
        [Required]
        public int CartId { get; set; }
        [Required]
        public Cart Cart { get; set; }
    }
}
