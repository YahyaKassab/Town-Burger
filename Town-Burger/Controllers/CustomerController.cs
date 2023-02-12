using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Town_Burger.Models;
using Town_Burger.Models.Dto;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IReviewService _reviewService;

        public CustomerController(ICustomerService customerService, IReviewService reviewService)
        {
            _customerService = customerService;
            _reviewService = reviewService;
        }

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
        [HttpPost("UpdateAddress")]
        public async Task<IActionResult> UpdateAddress(Address address)
        {
            // not adding
            var result = _customerService.UpdateAddress(address);
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

    }
}
