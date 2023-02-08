using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models
{
    public class Review
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Range(0, 5)]
        public double Rating { get; set; }
    }
}
