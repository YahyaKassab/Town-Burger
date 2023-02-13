using Microsoft.EntityFrameworkCore;
using Town_Burger.Models.Context;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface ISecondarySevice
    {
        Task<GenericResponse<string>> EditOrderingPolicies(string policies);
        Task<GenericResponse<string>> EditAboutUs(string aboutUs);
        Task<GenericResponse<string>> GetOrderingPolicies();
        Task<GenericResponse<string>> GetAboutUs();
    }
    public class SecondaryService: ISecondarySevice
    {
        private readonly AppDbContext _context;

        public SecondaryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResponse<string>> EditAboutUs(string aboutUs)
        {
            var secondary = await _context.Secondaries.FirstOrDefaultAsync();
            if (secondary == null)
            {
                return new GenericResponse<string>
                {
                    IsSuccess = false,
                    Message = "AboutUs doesnt exist"
                };
            }
            secondary.AboutUs = aboutUs;
            await _context.SaveChangesAsync();
            return new GenericResponse<string>
            {
                IsSuccess = true,
                Message = "Updated Successfully",
                Result = secondary.AboutUs
            };
        }

        public async Task<GenericResponse<string>> EditOrderingPolicies(string policies)
        {
            var secondary = await _context.Secondaries.FirstOrDefaultAsync();
            if (secondary == null)
            {
                return new GenericResponse<string>
                {
                    IsSuccess = false,
                    Message = "Policies doesnt exist"
                };
            }
            secondary.OrderingPolicies = policies;
            await _context.SaveChangesAsync();
            return new GenericResponse<string>
            {
                IsSuccess = true,
                Message = "Updated Successfully",
                Result = secondary.OrderingPolicies
            };
        }

        public async Task<GenericResponse<string>> GetAboutUs()
        {
            var secondary = await _context.Secondaries.FirstOrDefaultAsync();
            if (secondary == null)
                return new GenericResponse<string>
                {
                    IsSuccess = false,
                    Message = "No aboutUs found"
                };
            return new GenericResponse<string>
            {
                IsSuccess = true,
                Message = "About us fetched Successfully",
                Result = secondary.AboutUs.ToString()
            };
        }

        public async Task<GenericResponse<string>> GetOrderingPolicies()
        {
            var secondary = await _context.Secondaries.FirstOrDefaultAsync();
            if (secondary == null)
                return new GenericResponse<string>
                {
                    IsSuccess = false,
                    Message = "No Policies found"
                };
            return new GenericResponse<string>
            {
                IsSuccess = true,
                Message = "Policies fetched Successfully",
                Result = secondary.OrderingPolicies.ToString()
            };
        }
    }
}
