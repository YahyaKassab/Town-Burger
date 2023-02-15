using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Town_Burger.Models;
using Town_Burger.Models.Dto;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpGet("GetCartByCustomerId")]
        public async Task<IActionResult> Get(int Id)
        {
            var result = await _ordersService.GetCartByCustomerId(Id);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("UpdateCart")]
        public async Task<IActionResult> UpdateCart(int cartId, IEnumerable<UpdateCartItemDto> Items)
        {
            var result = await _ordersService.UpdateCartAsync(cartId, Items);
            if(result.IsSuccess )
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(int customerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _ordersService.PlaceOrder(customerId);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("ClearCart")]
        public async Task<IActionResult> Clear(int id)
        {
            var result = await _ordersService.clearCart(id);
            return Ok(result);

        }
    }
}
