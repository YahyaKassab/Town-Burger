using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Town_Burger.Models;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Identity;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IReviewService _reviewService;
        private readonly IOrdersService _ordersServics;
        private readonly ISecondarySevice _secondaryService;


        [ActivatorUtilitiesConstructor]
        public CustomerController(ICustomerService customerService, IReviewService reviewService, IOrdersService ordersServics, ISecondarySevice secondaryService)
        {
            _customerService = customerService;
            _reviewService = reviewService;
            _ordersServics = ordersServics;
            _secondaryService = secondaryService;
        }
        #region Customers 

        [HttpGet("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }


        [HttpPost("AddCustomer")]
        public async Task<IActionResult> RegisterCustomerAsync([FromBody] RegisterCustomerDto form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (form == null) return NotFound();
            var result = await _customerService.RegisterCustomerAsync(form);
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



        #endregion

        #region Address

        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddAddress(AddressDto address)
        {
            // not adding
            var result = await _customerService.AddAddressAsync(address);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("UpdateAddress")]
        public async Task<IActionResult> UpdateAddress(UpdateAddressDto address)
        {
            // not adding
            var result = await _customerService.UpdateAddress(address);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("DeleteAddress")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var result = await _customerService.DeleteAddressAsync(addressId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("GetAddressesByCustomerId")]
        public async Task<IActionResult> GetAddresses(int id)
        {
            var result = await _customerService.GetAddressesByCustomerId(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("GetAddressById")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var result = await _customerService.GetAddressById(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        #endregion

        #region Orders
        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders(int id)
        {
            var result = await _ordersServics.GetOrdersByCustomerId(id);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById(int id)
        {
           var result = await _ordersServics.GetOrderByIdAsync(id);
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("EditOrder")]
        public async Task<IActionResult> EditOrder(UpdateOrderDto order)
        {
            var result = await _ordersServics.EditOrder(order);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpDelete("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _ordersServics.DeleteOrder(id);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        #endregion

        #region Reviews
        [HttpGet("GetReviewById")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var result = await _reviewService.GetReviewById(id);
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview(ReviewDto model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _reviewService.AddReviewAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("EditReview")]
        public async Task<IActionResult> EditReview(Review review)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _reviewService.UpdateReview(review);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        [HttpDelete("DeleteReview")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _reviewService.DeleteReview(id);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        #endregion

        #region Secondaries
        [HttpGet("GetAboutUs")]

        public async Task<IActionResult> GetAboutUs()
        {
            var result = await _secondaryService.GetAboutUs();
            if(result.IsSuccess)
            {
                return Ok(result);  
            }
            return BadRequest(result);
        }
        [HttpGet("GetPolicies")]

        public async Task<IActionResult> GetPolicies()
        {
            var result = await _secondaryService.GetOrderingPolicies();
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        #endregion
    }
}
