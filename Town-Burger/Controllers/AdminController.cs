using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Town_Burger.Models.Identity;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public AdminController(IBalanceService balanceService, IUserService userService, UserManager<User> userManager)
        {
            _balanceService = balanceService;
            _userService = userService;
            _userManager = userManager;
        }


        #region Balance
        //[Authorize(Roles = "Admin")]
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


        [HttpPost("AddToBalance")]
        public async Task<IActionResult> AddToBalance(double amount)
        {
            var result = await _balanceService.AddToBalanceAsync(amount);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("SubFromBalance")]
        public async Task<IActionResult> SubFromBalance(double amount)
        {
            var result = await _balanceService.SubFromBalanceAsync(amount);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("AddDeposit")]
        public async Task<IActionResult> AddDeposit(string fromId, double amount)
        {
            var result = await _balanceService.AddDepositAsync(fromId, amount);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("AddSpend")]
        public async Task<IActionResult> AddSpend(string fromId, double amount)
        {
            var result = await _balanceService.AddSpendAsync(fromId, amount);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        //Day
        #region Day
        [HttpGet("GetDepositsDay")]
        public async Task<IActionResult> GetDepositsDay()
        {
            var result = await _balanceService.GetDepositsDay();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result.Result);
        }


        [HttpGet("GetSpendsDay")]
        public async Task<IActionResult> GetSpendsDay()
        {
            var result = await _balanceService.GetSpendsDay();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result.Result);
        }


        [HttpGet("GetEarningsDay")]
        public async Task<IActionResult> GetEarningsDay()
        {
            var result = await _balanceService.GetEarningsDay();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result.Result);
        }

        #endregion

        //Month
        #region Month
        [HttpGet("GetDepositsMonth")]
        public async Task<IActionResult> GetDepositsMonth()
        {
            var result = await _balanceService.GetDepositsMonth();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result.Result);
        }


        [HttpGet("GetSpendsMonth")]
        public async Task<IActionResult> GetSpendsMonth()
        {
            var result = await _balanceService.GetSpendsMonth();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result.Result);
        }


        [HttpGet("GetEarningsMonth")]
        public async Task<IActionResult> GetEarningsMonth()
        {
            var result = await _balanceService.GetEarningsMonth();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result.Result);
        }
        #endregion

        //Year
        #region Year
        [HttpGet("GetDepositsYear")]
        public async Task<IActionResult> GetDepositsYear()
        {
            var result = await _balanceService.GetDepositsYear();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result.Result);
        }
        [HttpGet("GetSpendsYear")]
        public async Task<IActionResult> GetSpendsYear()
        {
            var result = await _balanceService.GetSpendsYear();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result.Result);
        }
        [HttpGet("GetEarningsYear")]
        public async Task<IActionResult> GetEarningsYear()
        {
            var result = await _balanceService.GetEarningsYear();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result.Result);
        }
        #endregion
        #endregion

        #region Employees
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userManager.FindByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }


        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser(User user)
        {
            var result = await _userService.UpdateUserAsync(user);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }


        [HttpDelete("RemoveUser")]
        public async Task<IActionResult> RemoveUser(string userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }


        [HttpPost("AddToRole")]
        public async Task<IActionResult> AddToRole(string userId,string roleName)
        {
            var result = await _userService.AddToRole(userId,roleName);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("RemoveFromRole")]
        public async Task<IActionResult> RemoveFromRole(string userId, string roleName)
        {
            var result = await _userService.RemoveFromRole(userId, roleName);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        #endregion

    }
}
