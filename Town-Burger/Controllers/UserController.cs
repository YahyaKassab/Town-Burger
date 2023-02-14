using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Identity;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private IConfiguration _configuration;
        private readonly IEmployeeService _employeeService;
        private readonly ICustomerService _customerService;
        private readonly IMailService _mailService;
        private readonly AppDbContext _context;
        public UserController(IUserService userService, IEmployeeService employeeService, ICustomerService customerService, AppDbContext context, IMailService mailService, IConfiguration configuration)
        {
            _userService = userService;
            _employeeService = employeeService;
            _customerService = customerService;
            _context = context;
            _mailService = mailService;
            _configuration = configuration;
        }

        [HttpPost("AddCustomer")]
        public async Task<IActionResult> RegisterCustomerAsync([FromBody]RegisterCustomerDto form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (form == null) return NotFound();
            var result = await _customerService.RegisterCustomerAsync(form);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("LoginEmail")]
        public async Task<IActionResult> LoginAsync(LoginDto form)
        {
            if(!ModelState.IsValid) return BadRequest("Some Fields are not valid");
            var result = await _userService.LoginCustomerAsync(form);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return NotFound();

            var result = await _userService.ConfirmEmailAsync(userId, token);
            if (result.IsSuccess)
                return Redirect($"{_configuration["FrontUrl"]}/email-confirmed");

            return BadRequest(result);

        }

        [HttpGet("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var result = await _userService.ForgetPasswordAsync(email);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(model);
            var result = await _userService.ResetPasswordAsync(model);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }


        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            var result = await _customerService.UpdateCustomerAsync(customer);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }


        [HttpDelete("RemoveCustomer")]
        public async Task<IActionResult> RemoveCustomer(int cusomerId)
        {
            var result = await _customerService.DeleteCustomerAsync(cusomerId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail(SendEmailDto model)
        {
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);
            var result = await _mailService.SendViaMailKit(model);
            if(result)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }
    }
}
