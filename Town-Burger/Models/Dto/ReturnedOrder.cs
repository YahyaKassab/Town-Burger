using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models.Dto
{
    public class ReturnedOrder
    {
        public int Id { get; set; }
        public ReturnedCart? Cart { get; set; }
        public MyDate PlacedIn { get; set; }

        public MyDate? DeliveredIn { get; set; }

        public double? TotalPrice { get; set; }
        public int State { get; set; }

        public Address? Address { get; set; }
    }
}
