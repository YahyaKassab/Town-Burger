namespace Town_Burger.Models.Dto
{
    public class UpdateCartDto
    {
        public int Id { get; set; }
        public IEnumerable<UpdateCartItemDto> Items { get; set; }
    }
}
