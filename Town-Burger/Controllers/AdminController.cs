using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class AdminController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public AdminController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpGet("GetBalance")]
        public async Task<IActionResult> GetBalance()
        {
            var result = await _balanceService.GetBalance();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
