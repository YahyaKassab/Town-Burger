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
        //add from the parent table not from the child indepenedently 

        Task<GenericResponse<Address>> AddAddressAsync(AddressDto address);
        GenericResponse<Address> UpdateAddress(Address address);
        Task<GenericResponse<IEnumerable<Address>>> GetAddressesByCustomerId(int customerId);
        Task<GenericResponse<string>> DeleteAddressAsync(int addressId);
        Task<GenericResponse<Customer>> GetCustomerByIdAsync(int id);
        Task<GenericResponse<IEnumerable<Customer>>> GetAllCustomers();
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
            Customer customer = new Customer()
                {
                    FullName = form.FullName,
                    Cart = new Cart()
                    {
                        Items = null
                    }
                };
              
         

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

            var customer = await _context.Customers.Include(e=>e.Cart).FirstOrDefaultAsync(e => e.Id == customerId);


            if (customer == null)
                return new GenericResponse<string>
                {
                    IsSuccess = false,
                    Message = "Customer Not found"
                };

            var result =  _context.Remove(customer.Cart);
            _context.SaveChanges();
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
                var customer = await _context.Customers.Include(c=>c.Addresses).FirstOrDefaultAsync(c=>c.Id == address.CustomerId);
                var _address = new Address
                {
                    CustomerId = address.CustomerId,
                    Street = address.Street,
                    Details = address.Details,
                };
                customer.Addresses.Add(_address);
                await _context.SaveChangesAsync();
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

        public GenericResponse<Address> UpdateAddress(Address address)
        {
            try
            {
                var result = _context.Update(address);
                _context.SaveChanges();
                return new GenericResponse<Address>()
                {
                    IsSuccess = true,
                    Message = "Address Updated Successfully",
                    Result = address
                };
            }
            catch(Exception ex )
            {
                return new GenericResponse<Address>()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<GenericResponse<string>> DeleteAddressAsync(int addressId)
        {
            try
            {
                var address = await _context.Addresses.FindAsync(addressId);
                if (address == null)
                    return new GenericResponse<string>()
                    {
                        IsSuccess = false,
                        Message = "Address Doesnt exist"
                    };
                _context.Remove(address);
                await _context.SaveChangesAsync();
                return new GenericResponse<string>()
                {
                    IsSuccess = true,
                    Message = "Address Deleted Successfully"
                };

            }
            catch(Exception ex)
            {
                return new GenericResponse<string>()
                {
                    IsSuccess = false,
                    Message = "Delete Failed",
                    Result = ex.Message,
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<Customer>>> GetAllCustomers()
        {
            try
            {
                var customers = await _context.Customers.Include(c=>c.User).ToListAsync();
                if (customers.Count == 0)
                    return new GenericResponse<IEnumerable<Customer>>()
                    {
                        IsSuccess = false,
                        Message = "No Customers Found",
                    };
                return new GenericResponse<IEnumerable<Customer>>
                {
                    IsSuccess = true,
                    Message = "Customers fetched successfully",
                    Result = customers
                };
            }catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<Customer>>()
                {
                    IsSuccess = false,
                    Message = "There was an error"
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<Address>>> GetAddressesByCustomerId(int customerId)
        {
            var addresses = await _context.Addresses.Where(a=>a.CustomerId == customerId).ToListAsync();
            if (addresses.Count == 0)
                return new GenericResponse<IEnumerable<Address>>
                {
                    IsSuccess = false,
                    Message = "You dont have any addresses"
                };
            return new GenericResponse<IEnumerable<Address>>
            {
                IsSuccess = true,
                Message = "Addresses fetched successfully",
                Result = addresses
            };

        }
    }
}
