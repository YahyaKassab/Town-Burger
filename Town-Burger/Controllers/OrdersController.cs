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
        [HttpPost("UpdateCart")]
        public async Task<IActionResult> UpdateCart(Cart cart)
        {
            var result = await _ordersService.UpdateCartAsync(cart);
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
    }
}
