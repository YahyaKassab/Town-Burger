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
        
        
        Task<GenericResponse<User>> AddToRole(string userId, string roleName);
        Task<GenericResponse<User>> RemoveFromRole(string userId, string roleName);
        Task<GenericResponse<(string token, DateTime expire)>> LoginAsync(LoginDto form);
        Task<GenericResponse<string>> ConfirmEmailAsync(string email);
        Task<GenericResponse<string>> ForgetPasswordAsync(string email);
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


        public async Task<GenericResponse<string>> ConfirmEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<string>> ForgetPasswordAsync(string email)
        {
            throw new NotImplementedException();
        }


    }

}
