using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models.Dto
{
    public class ReturnedReview
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Range(0, 5)]
        public double Rating { get; set; }

        public DateTime Time { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }
    }
}
