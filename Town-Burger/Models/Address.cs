using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Town_Burger.Models.Identity;

namespace Town_Burger.Models
{
    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Street { get; set; }
        public string Details { get; set; }

        public User Customer { get; set; }
    }
}
