using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SendGrid.Helpers.Mail;
using System.Text;
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
        Task<GenericResponse<Address>> GetAddressById(int id);
        Task<GenericResponse<Address>> UpdateAddress(UpdateAddressDto address);
        Task<GenericResponse<IEnumerable<Address>>> GetAddressesByCustomerId(int customerId);
        Task<GenericResponse<string>> DeleteAddressAsync(int addressId);
        Task<GenericResponse<ReturnedCustomer>> GetCustomerByIdAsync(int id);
        Task<GenericResponse<IEnumerable<ReturnedCustomer>>> GetAllCustomers();
        Task<GenericResponse<IEnumerable<IdentityError>>> RegisterCustomerAsync(RegisterCustomerDto form);
        Task<GenericResponse<Customer>> UpdateCustomerAsync(Customer customer);
        Task<GenericResponse<string>> DeleteCustomerAsync(int customerId);
    }
    public class CustomerService:ICustomerService
    {
        private UserManager<User> _userManager;
        private IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IMailService _mailService;
        public CustomerService(UserManager<User> userManager, AppDbContext context, IMailService mailService)
        {
            _userManager = userManager;
            _context = context;
            _mailService = mailService;
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
            //check if email exists
            var _customer = await _userManager.FindByEmailAsync(form.Email);
            if (_customer != null)
                return new GenericResponse<IEnumerable<IdentityError>>
                {
                    IsSuccess = false,
                    Message = "Email Already used"
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


            var result = await _userManager.CreateAsync(user,form.Password);
            //succeeded
            await _userManager.AddToRoleAsync(user, "Customer");

            //send confirm email

            //generate the token

            var result2 = await _mailService.SendConfirmEmail(user.Email);
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

        public async Task<GenericResponse<ReturnedCustomer>> GetCustomerByIdAsync(int id)
        {
            if (id == null)
                return new GenericResponse<ReturnedCustomer>()
                {
                    IsSuccess = false,
                    Message = "Id is null"
                };
            var customer = await _context.Customers.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == id);
            if (customer == null)
                return new GenericResponse<ReturnedCustomer>()
                {
                    IsSuccess = false,
                    Message = "Customer Doesnt exist"
                };
            return new GenericResponse<ReturnedCustomer>()
            {
                IsSuccess = true,
                Message = "Customer fetched successfully",
                Result = new ReturnedCustomer
                {
                    Id= id,
                    Email = customer.User.Email,
                    FullName = customer.FullName,
                    PhoneNumber = customer.User.PhoneNumber,
                }
            };
        }
        public async Task<GenericResponse<string>> DeleteCustomerAsync(int customerId)
        {

            var customer = await _context.Customers.Include(e=>e.Cart).ThenInclude(c=>c.Items).Include(c=>c.User).Include(c=>c.Addresses).Include(c=>c.Reviews).Include(c=>c.DepositsCustomer).FirstOrDefaultAsync(e => e.Id == customerId);
            var user = customer.User;

            if (customer == null)
                return new GenericResponse<string>
                {
                    IsSuccess = false,
                    Message = "Customer Not found"
                };
            if (customer.Cart != null)
            {
                //delete the cart
                _context.RemoveRange(customer.Cart.Items);
                _context.Remove(customer.Cart);
            }
            if(customer.Addresses.Count > 0)
            {
                foreach(var address in customer.Addresses)
                {
                    //deletes addresses and orders
                    var result = await DeleteAddressAsync(address.Id);
                    if (!result.IsSuccess)
                        return new GenericResponse<string> { IsSuccess = false, Message = "failed to delete the Address" };
                }
            }
            if(customer.Reviews.Count > 0)
            {
                //delete the reviews
                _context.RemoveRange(customer.Reviews);
            }
            if(customer.DepositsCustomer.Count > 0)
            {
                //delete the deposits
                _context.RemoveRange(customer.DepositsCustomer);

            }

            await _context.SaveChangesAsync();
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

        public async Task<GenericResponse<Address>> UpdateAddress(UpdateAddressDto address)
        {
            try
            {
                var _address = await _context.Addresses.FindAsync(address.Id);
                _address.Street = address.Street;
                if(address.Details != null)
                    _address.Details = address.Details;

                await _context.SaveChangesAsync();
                return new GenericResponse<Address>()
                {
                    IsSuccess = true,
                    Message = "Address Updated Successfully",
                    Result = _address
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
                var address = await _context.Addresses.Include(a=>a.Orders).ThenInclude(o=>o.CartItems).FirstOrDefaultAsync(a=>a.Id == addressId);
                if (address == null)
                    return new GenericResponse<string>()
                    {
                        IsSuccess = false,
                        Message = "Address Doesnt exist"
                    };
                if(address.Orders.Count > 0)
                {
                    //delet orders if exist
                    foreach (var order in address.Orders)
                    {
                        if(order.CartItems.Count > 0)
                        {
                            _context.RemoveRange(order.CartItems);
                            order.CartItems.Clear();
                        }
                        _context.Remove(order);
                    }
                }

                address.Orders.Clear();
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

        public async Task<GenericResponse<IEnumerable<ReturnedCustomer>>> GetAllCustomers()
        {
            try
            {
                var customers = await _context.Customers.Include(c=>c.User).ToListAsync();
                if (customers.Count == 0)
                    return new GenericResponse<IEnumerable<ReturnedCustomer>>()
                    {
                        IsSuccess = false,
                        Message = "No Customers Found",
                    };

                var returnedCustomers = new List<ReturnedCustomer>();
                foreach (var customer in customers)
                {
                    returnedCustomers.Add(new ReturnedCustomer
                    {
                        Id = customer.Id,
                        Email = customer.User.Email,
                        FullName = customer.FullName,
                        PhoneNumber = customer.User.PhoneNumber
                    });
                }
                return new GenericResponse<IEnumerable<ReturnedCustomer>>
                {
                    IsSuccess = true,
                    Message = "Customers fetched successfully",
                    Result = returnedCustomers
                };
            }catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<ReturnedCustomer>>()
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
                    IsSuccess = true,
                    Message = "You dont have any addresses"
                };
            return new GenericResponse<IEnumerable<Address>>
            {
                IsSuccess = true,
                Message = "Addresses fetched successfully",
                Result = addresses
            };

        }

        public async Task<GenericResponse<Address>> GetAddressById(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return new GenericResponse<Address>()
                {
                    IsSuccess = false,
                    Message = "Address Not Found"
                };
            return new GenericResponse<Address>()
            {
                IsSuccess = true,
                Message = "Address Fetched Successfully",
                Result = address
            };
        }
    }
}
