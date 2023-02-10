using Microsoft.AspNetCore.Identity;

namespace Town_Burger.Models.Identity
{
    public class User:IdentityUser
    {
        public Customer? Customer{ get; set; }
        public Employee? Employee{ get; set; }
    }
}
