namespace Town_Burger.Models.Dto
{
    public class ReturnedCart
    {
        public int Id { get; set; }

        public IEnumerable<ReturnedCartItem> Items { get; set; }
    }
}
