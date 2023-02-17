using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        private readonly AppDbContext _context;

        public OrdersController(IOrdersService ordersService, AppDbContext context)
        {
            _ordersService = ordersService;
            _context = context;
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
        public async Task<IActionResult> UpdateCart(UpdateCartDto model)
        {
            var result = await _ordersService.UpdateCartAsync(model);
            if(result.IsSuccess )
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(int addressId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _ordersService.PlaceOrder(addressId);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("clearcart")]
        public async Task<IActionResult> clearCart()
        {
         var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == 34);
            cart.Items.Clear();
            return Ok(cart);
        }
    }
}
