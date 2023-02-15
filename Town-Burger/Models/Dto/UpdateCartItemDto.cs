namespace Town_Burger.Models.Dto
{
    public class UpdateCartItemDto
    {
        public int ItemId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}
