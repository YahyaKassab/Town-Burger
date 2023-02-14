using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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
        //add from the parent table not from the child indepenedently 



        Task<GenericResponse<User>> AddToRole(string userId, string roleName);
        Task<GenericResponse<User>> RemoveFromRole(string userId, string roleName);
        Task<GenericResponse<ReturnedCustomer>> LoginCustomerAsync(LoginDto form);
        Task<GenericResponse<string>> ConfirmEmailAsync(string userId, string token);
        Task<GenericResponse<string>> ForgetPasswordAsync(string email);
        Task<GenericResponse<string>> ResetPasswordAsync(ResetPasswordDto model);
        Task<GenericResponse<string>> DeleteUserAsync(string userId);
    }
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICustomerService _customerService;
        private IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IMailService _mailService;
        public UserService(UserManager<User> userManager, IConfiguration configuration, AppDbContext context, IMailService mailService, ICustomerService customerService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _mailService = mailService;
            _customerService = customerService;
        }



        //modify customer and employee services


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
            if (result.Succeeded)
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

        public async Task<GenericResponse<ReturnedCustomer>> LoginCustomerAsync(LoginDto form)
        {
            var user = await _userManager.FindByEmailAsync(form.Email);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (user == null) return new GenericResponse<ReturnedCustomer>()
            {
                IsSuccess = false,
                Message = "Email Not Found"
            };
            //user exists

            var result = await _userManager.CheckPasswordAsync(user, form.Password);
            if (!result)
                return new GenericResponse<ReturnedCustomer>()
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
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return new GenericResponse<ReturnedCustomer>()
            {
                IsSuccess = true,
                Message = "Logged In Successfully",
                Result = new ReturnedCustomer
                {
                        FullName = customer.FullName,
                        Email = user.Email,
                        Id = customer.Id,
                        PhoneNumber = user.PhoneNumber,
                        Token = tokenAsString,
                        ExpireDate = token.ValidTo
                }
            };

        }


        public async Task<GenericResponse<string>> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GenericResponse<string> { IsSuccess = false, Message = "User Not Found" };

            //decode the token
            var decodedToken = WebEncoders.Base64UrlDecode(token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new GenericResponse<string>
                {
                    IsSuccess = true,
                    Message = "Email Confirmed Successfully"
                };
            return new GenericResponse<string> { IsSuccess = false, Message = "Error Confirming the Email", Errors = result.Errors.Select(e => e.Description).ToArray() };



        }

        public async Task<GenericResponse<string>> ForgetPasswordAsync(string email)
        {
            //check if there is a user
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new GenericResponse<string> { IsSuccess = false, Message = "User Not Found" };
            //user exists

            var result = await _mailService.SendResetPasswordEmail(email);
            if (result.IsSuccess)
                return new GenericResponse<string>
                {
                    IsSuccess = true,
                    Message = "Email Sent Successfully"
                };
            return new GenericResponse<string>
            {
                IsSuccess = false,
                Message = result.Message
            };

        }

        public async Task<GenericResponse<string>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            try
            {
                await _userManager.DeleteAsync(user);
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
                    Message = "failed To Delete the User",
                    Errors = new[] { ex.Message.ToString() }
                };
            }
        }

        public async Task<GenericResponse<string>> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new GenericResponse<string> { IsSuccess = false, Message = "User Not found" };


            //decode the token
            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.Password);

            if (result.Succeeded)
                return new GenericResponse<string>
                {
                    IsSuccess = true,
                    Message = "Password Changed Successfully"
                };
            return new GenericResponse<string>()
            {
                IsSuccess = false,
                Message = "Failed To Reset the password",
                Errors = result.Errors.Select(e => e.Description).ToArray(),
            };
        }
    }

}
