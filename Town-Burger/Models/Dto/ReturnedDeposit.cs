using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Identity;

namespace Town_Burger.Models.Dto
{
    public class ReturnedDeposit
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime Time { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerPhone { get; set; }

        public int CustomerId { get; set;}
    }
}
