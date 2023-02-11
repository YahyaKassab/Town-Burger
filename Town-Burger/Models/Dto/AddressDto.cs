using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models.Dto
{
    public class AddressDto
    {
        [Required]
        public string Street{ get; set; }
        [Required]
        public string Details{ get; set; }
        public int CustomerId { get; set; }
    }
}
