using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Town_Burger.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public MenuItem Item { get; set; }

        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int? OrderId { get; set; }

        public Order? Order { get; set; }
        public int? CartId { get; set; }

        public Cart? Cart { get; set; }
    }
}
