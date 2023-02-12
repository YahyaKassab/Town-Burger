using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public EmployeeController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpPost("UpdateState")]
        public async Task<IActionResult> UpdateState(int orderId, int state)
        {
            var result = await _ordersService.UpdateState(orderId, state);
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
