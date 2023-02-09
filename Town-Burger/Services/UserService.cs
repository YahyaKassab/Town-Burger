using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Mail;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        Task<GenericResponse<User>> AddToRole(string userId, string roleName);
        Task<GenericResponse<User>> RemoveFromRole(string userId, string roleName);
        Task<GenericResponse<User>> UpdateUserAsync(User user);
        Task<GenericResponse<string>> DeleteUserAsync(string userId);
        Task<GenericResponse<string>> ConfirmEmailAsync(string email);
        Task<GenericResponse<string>> ForgetPasswordAsync(string email);
        Task<GenericResponse<(string token, DateTime expire)>> LoginAsync(LoginDto form);
    }
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
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
            if (form.Address != null)
                customer = new User()
                {
                    FullName = form.FullName,
                    Email = form.Email,
                    PhoneNumber = form.PhoneNumber,
                    Addresses = new Address[] { form.Address }
                };
            else 
                customer = new User() { 
                FullName = form.FullName,
                Email = form.Email,
                PhoneNumber = form.PhoneNumber,
                };

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
                    employee = new User()
                    {
                        FullName = form.FullName,
                        Email = form.Email,
                        PhoneNumber = form.PhoneNumber,
                        Salary = form.Salary,
                        ContractBegins = form.ContractBegins,
                        ContractEnds = form.ContractEnds
                    };
                else
                    employee = new User() 
                    {
                        FullName = form.FullName,
                        Email = form.Email,
                        PhoneNumber = form.PhoneNumber,
                        Salary = form.Salary,
                        ContractBegins = form.ContractBegins,
                        ContractEnds = form.ContractEnds,
                        DaysOfWork = form.DaysOfWork 
                    };
            }
            else
            {
                if (form.DaysOfWork == null)
                    employee = new User()
                    {
                        FullName = form.FullName,
                        Email = form.Email,
                        PhoneNumber = form.PhoneNumber,
                        Salary = form.Salary,
                        ContractBegins = form.ContractBegins,
                        ContractEnds = form.ContractEnds,
                        Picture = form.Picture
                    };
                else
                    employee = new User()
                    {
                        FullName = form.FullName,
                        Email = form.Email,
                        PhoneNumber = form.PhoneNumber,
                        Salary = form.Salary,
                        ContractBegins = form.ContractBegins,
                        ContractEnds = form.ContractEnds,
                        DaysOfWork = form.DaysOfWork,
                        Picture = form.Picture
                    };
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

        public async Task<GenericResponse<(string token,DateTime expire)>> LoginAsync(LoginDto form)
        {
            var user = await _userManager.FindByEmailAsync(form.Email);
            if (user == null) return new GenericResponse<(string token, DateTime expire)>()
            {
                IsSuccess = false,
                Message = "Email Not Found"
            };
            //user exists
            if(form.Password != form.ConfirmPassword)
                return new GenericResponse<(string token, DateTime expire)>()
                {
                    IsSuccess = false,
                    Message = "Passwords dont match"
                };
            //password match and user exists
            var result = await _userManager.CheckPasswordAsync(user, form.Password);
            if(!result)
                return new GenericResponse<(string token, DateTime expire)>()
                {
                    IsSuccess = false,
                    Message = "Wrong Password"
                };
            //Generate token and login
            var roles = await _userManager.GetRolesAsync(user);
            var rolesClaims = new List<Claim>();
            foreach (var role in roles)
                rolesClaims.Add(new Claim(ClaimTypes.Role, role));
            var roleClaimsAsArray = rolesClaims.ToArray();

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Email,form.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.MobilePhone,user.PhoneNumber),
                
            }.Concat(roleClaimsAsArray);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials:new SigningCredentials(key,SecurityAlgorithms.HmacSha256)
                );
            string tokenAsString =  new JwtSecurityTokenHandler().WriteToken(token);
            return new GenericResponse<(string token, DateTime expire)>()
            {
                IsSuccess = true,
                Message = "Logged In Successfully",
                Result = (tokenAsString,token.ValidTo)
            };

        }

        public async Task<GenericResponse<User>> UpdateUserAsync(User user)
        {
            if (user == null)
                return new GenericResponse<User>()
                {
                    IsSuccess = false,
                    Message = "User Is null"
                };
            try
            {
                _context.Entry(user).State = EntityState.Detached;
                var result = _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return new GenericResponse<User>()
                {
                    IsSuccess = true,
                    Message = "User Updated Successfully",
                    Result = user
                };
            }catch(Exception ex)
            {
                return new GenericResponse<User>()
                {
                    IsSuccess = false,
                    Message = "failed To Update the user",
                    Errors = new[] { ex.Message.ToString() }
                };
            }
        }

        public async Task<GenericResponse<string>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GenericResponse<string>()
                {
                    IsSuccess = false,
                    Message = "User Is null"
                };
            try
            {
                var result = await _userManager.DeleteAsync(user);
                return new GenericResponse<string>()
                {
                    IsSuccess = true,
                    Message = "User Deleted Successfully",
                    Result = "Success"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<string>()
                {
                    IsSuccess = false,
                    Message = "failed To Delete the user",
                    Errors = new[] { ex.Message.ToString() }
                };
            }
        }

        public async Task<GenericResponse<string>> ConfirmEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<string>> ForgetPasswordAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<User>> AddToRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || roleName == null)
                return new GenericResponse<User>
                {
                    IsSuccess = false,
                    Message = "User or RoleName are null"
                };
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if(result.Succeeded)
            {
                return new GenericResponse<User>
                {
                    IsSuccess = true,
                    Message = $"User Added To Role {roleName.ToUpper()} Successfully",
                    Result = user
                };
            }
            return new GenericResponse<User>
            {
                IsSuccess = false,
                Message = "Failed To add to the role"
            };
        }

        public async Task<GenericResponse<User>> RemoveFromRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || roleName == null)
                return new GenericResponse<User>
                {
                    IsSuccess = false,
                    Message = "User or RoleName are null"
                };
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return new GenericResponse<User>
                {
                    IsSuccess = true,
                    Message = $"User Removed From Role {roleName.ToUpper()} Successfully",
                    Result = user
                };
            }
            return new GenericResponse<User>
            {
                IsSuccess = false,
                Message = "Failed To Remove from the role"
            };
        }
    }

}
