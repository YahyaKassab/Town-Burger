using Town_Burger.Models.Identity;

namespace Town_Burger.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string Details { get; set; }

        public User Customer { get; set; }
    }
}
