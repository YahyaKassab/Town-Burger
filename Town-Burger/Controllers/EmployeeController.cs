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
        private readonly IBalanceService _balanceService;

        public EmployeeController(IOrdersService ordersService, IBalanceService balanceService)
        {
            _ordersService = ordersService;
            _balanceService = balanceService;
        }

        [HttpPut("UpdateState")]
        public async Task<IActionResult> UpdateState(int orderId, int state)
        {
            var result = await _ordersService.UpdateState(orderId, state);
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddDeposit")]
        public async Task<IActionResult> AddDeposit(int fromId, double amount)
        {
            var result = await _balanceService.AddDepositAsync(fromId, amount);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("DeleteDeposit")]
        public async Task<IActionResult> DeleteDeposit(int id)
        {
            var result = await _balanceService.DeleteDeposit(id);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpDelete("DeleteSpend")]
        public async Task<IActionResult> DeleteSpendt(int id)
        {
            var result = await _balanceService.DeleteSpend(id);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddSpend")]
        public async Task<IActionResult> AddSpend(int fromId, double amount)
        {
            var result = await _balanceService.AddSpendAsync(fromId, amount);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

    }
}
