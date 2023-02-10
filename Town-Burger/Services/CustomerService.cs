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
            User user = new User()
            {
                Email = form.Email,
                UserName = form.Email,
                PhoneNumber = form.PhoneNumber,
            };
            var email = await _userManager.FindByEmailAsync(user.Email);
            if (email != null)
                return new GenericResponse<IEnumerable<IdentityError>>
                {
                    IsSuccess = false,
                    Message = "Email Already Exists"
                };
            var username = await _userManager.FindByNameAsync(user.UserName);
            if (username != null)
                return new GenericResponse<IEnumerable<IdentityError>>
                {
                    IsSuccess = false,
                    Message = "Username Already Exists"
                };

            Customer customer;
            if (form.Address == null)
            {
                customer = new Customer()
                {
                    FullName = form.FullName,
                    User = user,
                };
            }
            else
            {
                customer = new Customer()
                {
                    FullName = form.FullName,
                    User = user,
                    Addresses = new Address[] {form.Address}
                };
            }
            //succeeded
            
            var result2 = await _context.Customers.AddAsync(customer);
            var result3 = await _context.SaveChangesAsync();
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
                var result = _context.Customers.Update(customer);
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
                return new GenericResponse<string>()
                {
                    IsSuccess = false,
                    Message = "Customer Is null"
                };
            var user = await _userManager.FindByIdAsync(customer.UserId);
            try
            {
                var result = _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                await _userManager.DeleteAsync(user);
                return new GenericResponse<string>()
                {
                    IsSuccess = true,
                    Message = "Customer Deleted Successfully",
                    Result = "Employee"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<string>()
                {
                    IsSuccess = false,
                    Message = "failed To Delete the Employee",
                    Errors = new[] { ex.Message.ToString() }
                };
            }
        }
    }
}
