using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Identity;

namespace Town_Burger.Models
{
   
    public class Order
    {

        public int Id { get; set; }
        [Required]
        public Cart Cart { get; set; }
        [Required]
        public DateTime PlacedIn { get; set; }

        public DateTime? DeliveredIn { get; set; }

        public int State { get; set; } = 0;

        public int CustomerId { get; set; }
    }

  

}
