using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models
{
   
    public class Order
    {

        public int Id { get; set; }
        [Required]
        public Cart cart { get; set; }
        [Required]
        public DateTime PlacedIn { get; set; }

        public DateTime? DeliveredIn { get; set; }

        public int State { get; set; } = 0;
    }

  

}
