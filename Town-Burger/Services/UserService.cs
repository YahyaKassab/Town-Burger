using Microsoft.AspNetCore.Identity;
using SendGrid.Helpers.Mail;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Identity;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IUserService
    {
        Task<GenericResponse<IEnumerable<IdentityError>>> RegisterCustomerAsync(RegisterCustomerDto form);
        Task<GenericResponse<IEnumerable<IdentityError>>> RegisterEmployeeAsync(RegisterEmployeeDto form);
    }
    public class UserService : IUserService
    {
        private UserManager<User> _userManager;
        private IConfiguration _configuration;
        private readonly AppDbContext _context;
        public UserService(UserManager<User> userManager, IConfiguration configuration, AppDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
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
            User customer;
            if(form.Address != null)
                customer = new User(form.FullName,form.Email,form.PhoneNumber,new Address[] {form.Address});
            else 
                customer = new User(form.FullName, form.Email, form.PhoneNumber);

            var result = await _userManager.CreateAsync(customer, form.Password);
            if (!result.Succeeded)
            {

                return new GenericResponse<IEnumerable<IdentityError>>()
                {
                    IsSuccess = false,
                    Message = "Failed To Create The User",
                    Result = result.Errors
                };
            }

            //succeeded
            await _userManager.AddToRoleAsync(customer, _configuration["DefaultRole"]);
            return new GenericResponse<IEnumerable<IdentityError>>()
            {
                IsSuccess = true,
                Message = "User Created Successfully",
            };
        }

        public async Task<GenericResponse<IEnumerable<IdentityError>>> RegisterEmployeeAsync(RegisterEmployeeDto form)
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
            User employee;
            if(form.Picture == null)
            {
                if(form.DaysOfWork == null)
                    employee = new User(form.FullName,form.Email,form.PhoneNumber,form.Salary,form.ContractBegins,form.ContractEnds);
                else
                    employee = new User(form.FullName,form.Email,form.PhoneNumber,form.Salary,form.ContractBegins,form.ContractEnds,form.DaysOfWork);
            }
            else
            {
                if (form.DaysOfWork == null)
                    employee = new User(form.FullName, form.Email, form.PhoneNumber, form.Salary, form.ContractBegins, form.ContractEnds,null,form.Picture);
                else
                    employee = new User(form.FullName, form.Email, form.PhoneNumber, form.Salary, form.ContractBegins, form.ContractEnds, form.DaysOfWork,form.Picture);
            }
            var result = await _userManager.CreateAsync(employee, form.Password);
            if (!result.Succeeded)
            {

                return new GenericResponse<IEnumerable<IdentityError>>()
                {
                    IsSuccess = false,
                    Message = "Failed To Create The User",
                    Result = result.Errors
                };
            }

            //succeeded
            await _userManager.AddToRoleAsync(employee, "Employee");
            return new GenericResponse<IEnumerable<IdentityError>>()
            {
                IsSuccess = true,
                Message = "User Created Successfully",
            };
        }
    }

}
