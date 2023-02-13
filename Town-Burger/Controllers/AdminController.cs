using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
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
        private readonly IEmployeeService _employeeService;
        private readonly ICustomerService _customerService;
        private readonly AppDbContext _context;
        private readonly IOrdersService _orderService;
        private readonly IMenuService _menuService;
        private readonly IReviewService _reviewService;
        private readonly ISecondarySevice _secondaryService;


        public AdminController(IBalanceService balanceService, IUserService userService, UserManager<User> userManager, IEmployeeService employeeService, AppDbContext context, IOrdersService orderService, IMenuService menuService, IReviewService reviewService, ISecondarySevice secondaryService, ICustomerService customerService)
        {
            _balanceService = balanceService;
            _userService = userService;
            _userManager = userManager;
            _employeeService = employeeService;
            _context = context;
            _orderService = orderService;
            _menuService = menuService;
            _reviewService = reviewService;
            _secondaryService = secondaryService;
            _customerService = customerService;
        }
        //ok
        #region Menu
        [HttpPut("UpdateItem")]
        public IActionResult UpdateItem(MenuItem item)
        {
            var result = _menuService.UpdateMenuItem(item);
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpDelete("DeleteItem")]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            var result = await _menuService.DeleteMenuItem(itemId);
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        #endregion

        //ok
        #region Reviews

        [HttpGet("GetAllReviews")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _reviewService.GetAll();
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("GetLatestReviews")]
        public async Task<IActionResult> GetLatest()
        {
            var result = await _reviewService.GetLatest();
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        #endregion

        //ok
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


        [HttpPut("AddToBalance")]
        public async Task<IActionResult> AddToBalance(double amount)
        {
            var result = await _balanceService.AddToBalanceAsync(amount);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPut("SubFromBalance")]
        public async Task<IActionResult> SubFromBalance(double amount)
        {
            var result = await _balanceService.SubFromBalanceAsync(amount);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("GetSpendById")]
        public async Task<IActionResult> GetSpendById(int id)
        {
            var result = await _balanceService.GetSpendById(id);
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("GetDepositById")]
        public async Task<IActionResult> GetDepositById(int id)
        {
            var result = await _balanceService.GetDepositById(id);
            if(result.IsSuccess)
                return Ok(result);
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

        //Total
        #region Total
        [HttpGet("GetSpendsTotal")]
        public async Task<IActionResult> GetSpendsTotal()
        {
            var result = await _balanceService.GetSpendsTotal();
            if (result.IsSuccess)
                return Ok(result.Result);
            return BadRequest(result);


        }
        [HttpGet("GetDepositsTotal")]
        public async Task<IActionResult> GetDepositsTotal()
        {
            var result = await _balanceService.GetDepositsTotal();
            if (result.IsSuccess)
                return Ok(result.Result);
            return BadRequest(result);


        }
        [HttpGet("GetEarningsTotal")]
        public async Task<IActionResult> GetEarningsTotal()
        {
            var result = await _balanceService.GetEarningsTotal();
            if (result.IsSuccess)
                return Ok(result.Result);
            return BadRequest(result);


        }

        #endregion
        #endregion

        //ok
        #region Employees
        [HttpPost("AddEmployee")]
        public async Task<IActionResult> RegisterEmployeeAsync(RegisterEmployeeDto form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _employeeService.RegisterEmployeeAsync(form);
            if (form == null) return NotFound();
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userManager.FindByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        
        [HttpGet("GetEmployeeById")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var result = await _employeeService.GetAllEmployees();
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
            var result = await _employeeService.UpdateEmployeeAsync(employee);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }


        [HttpDelete("RemoveEmployee")]
        public async Task<IActionResult> RemoveEmployee(int employeeId)
        {
            var result = await _employeeService.DeleteEmployeeAsync(employeeId);
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

        //ok
        #region Customers
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _customerService.GetAllCustomers();
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        #endregion

        //ok
        #region Orders 
        [HttpGet("GetMostOrdered")]
        public async Task<IActionResult> GetMostOrdered()
        {
            var result = await _orderService.GetMostOrdered();
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var result = await _orderService.GetOrderByIdAsync(orderId);
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        #endregion

        //ok
        #region Secondary

        [HttpPut("EditAboutUs")]
        public async Task<IActionResult> EditAboutUs(string aboutUs)
        {
            var result = await _secondaryService.EditAboutUs(aboutUs);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPut("EditPolices")]
        public async Task<IActionResult> EditPolices(string policies)
        {
            var result = await _secondaryService.EditOrderingPolicies(policies);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("GetAboutUs")]
        public async Task<IActionResult> GetAboutUs()
        {
            var result = await _secondaryService.GetAboutUs();
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("GetPolices")]
        public async Task<IActionResult> GetPolices()
        {
            var result = await _secondaryService.GetOrderingPolicies();
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        #endregion

    }
}
