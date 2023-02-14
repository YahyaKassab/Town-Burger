namespace Town_Burger.Models.Dto
{
    public class ReturnedCustomer
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Token { get; set; }

        public DateTime ExpireDate { get; set; }
    }
}
