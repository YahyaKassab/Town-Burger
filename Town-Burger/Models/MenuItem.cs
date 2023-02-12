using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Title { get; set; }
        [Required]
        public string Type { get; set; }
        public string Description { get; set; }
        public string? ImageSource{ get; set; }
        [Required]
        public double Price { get; set; }
//        public ICollection<CartItem>? CartItems { get; set; }


    }
}
