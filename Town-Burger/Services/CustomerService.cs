using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Identity;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface ICustomerService
    {
        Task<GenericResponse<Address>> AddAddressAsync(AddressDto address);
        Task<GenericResponse<Customer>> GetCustomerByIdAsync(int id);
        Task<GenericResponse<IEnumerable<IdentityError>>> RegisterCustomerAsync(RegisterCustomerDto form);
        Task<GenericResponse<Customer>> UpdateCustomerAsync(Customer customer);
        Task<GenericResponse<string>> DeleteCustomerAsync(int customerId);
    }
    public class CustomerService:ICustomerService
    {
        private UserManager<User> _userManager;
        private readonly AppDbContext _context;
        public CustomerService(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<GenericResponse<IEnumerable<IdentityError>>> RegisterCustomerAsync(RegisterCustomerDto form)
        {

            if (form == null)
                return new GenericResponse<IEnumerable<IdentityError>>()
                {
                    IsSuccess = false,
                    Message = "Some Required Fields are left empty"
                };

            if (form.Password != form.ConfirmPassword)
                return new GenericResponse<IEnumerable<IdentityError>>
                {
                    IsSuccess = false,
                    Message = "Passwords dont match"
                };

            //form isnt null
            //passwords match

            //my customer
            Customer customer;
            if (form.Address == null)
            {
                customer = new Customer()
                {
                    FullName = form.FullName,
                    Cart = new Cart()
                    };
              
            }
            else
            {
                customer = new Customer()
                {
                    FullName = form.FullName,
                    Addresses = new Address[] {form.Address},
                    Cart = new Cart()
                };
            }

            //create the user along with the employee
            User user = new User()
            {
                Email = form.Email,
                UserName = form.Email,
                PhoneNumber = form.PhoneNumber,
                Customer = customer
            };


            var result = await _userManager.CreateAsync(user);
            //succeeded
            await _userManager.AddToRoleAsync(user, "Customer");
            return new GenericResponse<IEnumerable<IdentityError>>()
            {
                IsSuccess = true,
                Message = "Customer Created Successfully",
            };
        }
        public async Task<GenericResponse<Customer>> UpdateCustomerAsync(Customer customer)
        {
            if (customer == null)
                return new GenericResponse<Customer>()
                {
                    IsSuccess = false,
                    Message = "Customer Is null"
                };
            try
            {
                var result = _context.Update(customer);
                await _context.SaveChangesAsync();
                return new GenericResponse<Customer>()
                {
                    IsSuccess = true,
                    Message = "Customer Updated Successfully",
                    Result = customer
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Customer>()
                {
                    IsSuccess = false,
                    Message = "failed To Update the user",
                    Errors = new[] { ex.Message.ToString() }
                };
            }
        }

        public async Task<GenericResponse<Customer>> GetCustomerByIdAsync(int id)
        {
            if (id == null)
                return new GenericResponse<Customer>()
                {
                    IsSuccess = false,
                    Message = "Id is null"
                };
            var result = await _context.Customers.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == id);
            if (result == null)
                return new GenericResponse<Customer>()
                {
                    IsSuccess = false,
                    Message = "Customer Doesnt exist"
                };
            return new GenericResponse<Customer>()
            {
                IsSuccess = true,
                Message = "Customer fetched successfully",
                Result = result
            };
        }
        public async Task<GenericResponse<string>> DeleteCustomerAsync(int customerId)
        {

            var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == customerId);


            if (customer == null)
                return new GenericResponse<string>
                {
                    IsSuccess = false,
                    Message = "Customer Not found"
                };


            var user = await _userManager.FindByIdAsync(customer.UserId);

            try
            {
                await _userManager.DeleteAsync(user);
                return new GenericResponse<string>()
                {
                    IsSuccess = true,
                    Message = "Customer Deleted Successfully",
                    Result = "Success"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<string>()
                {
                    IsSuccess = false,
                    Message = "failed To Delete the Customer",
                    Errors = new[] { ex.Message.ToString() }
                };
            }
        }

        public async Task<GenericResponse<Address>> AddAddressAsync(AddressDto address)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(address.CustomerId);
                var _address = new Address
                {
                    CustomerId = address.CustomerId,
                    Customer = customer,
                    Street = address.Street,
                    Details = address.Details,
                };
                var result = await _context.Addresses.AddAsync(_address);
                return new GenericResponse<Address>()
                {
                    IsSuccess = true,
                    Message = "Address Added Successfully",
                    Result = _address
                };
            }catch(Exception ex)
            {
                return new GenericResponse<Address>()
                {
                    IsSuccess = false,
                    Message = "Failed to add the address"
                };
            }

        }
    }
}
