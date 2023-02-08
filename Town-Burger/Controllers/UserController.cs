using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Town_Burger.Models.Dto;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("AddCustomer")]
        public async Task<IActionResult> RegisterCustomerAsync(RegisterCustomerDto form)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.RegisterCustomerAsync(form);
            if(form == null) return NotFound();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
