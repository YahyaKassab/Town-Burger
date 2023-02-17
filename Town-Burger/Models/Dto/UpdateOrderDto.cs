namespace Town_Burger.Models.Dto
{
    public class UpdateOrderDto
    {
        public int Id { get; set; }
        public IEnumerable<UpdateCartItemDto>? Items { get; set; }
        public int? AddressId { get; set; }
    }
}
