using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Identity;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IEmployeeService
    {
        Task<GenericResponse<Employee>> GetEmployeeByIdAsync(int id);
        Task<GenericResponse<IEnumerable<IdentityError>>> RegisterEmployeeAsync(RegisterEmployeeDto form);
        Task<GenericResponse<Employee>> UpdateEmployeeAsync(Employee employee);
        Task<GenericResponse<string>> DeleteEmployeeAsync(int employeeId);
    }
    public class EmployeeService : IEmployeeService
    {
        private UserManager<User> _userManager;
        private readonly AppDbContext _context;
        public EmployeeService(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
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
            User user = new User()
            {
                Email= form.Email,
                UserName = form.Email,
                PhoneNumber= form.PhoneNumber,
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

            Employee employee;
            if (form.PictureSource == null)
            {
                if (form.DaysOfWork == null)
                    employee = new Employee()
                    {
                        FullName = form.FullName,
                        Salary = form.Salary,
                        ContractBegins = form.ContractBegins,
                        ContractEnds = form.ContractEnds,
                        User = user,
                    };
                else
                    employee = new Employee()
                    {
                        FullName = form.FullName,
                        Salary = form.Salary,
                        ContractBegins = form.ContractBegins,
                        ContractEnds = form.ContractEnds,
                        DaysOfWork = form.DaysOfWork,
                        User = user,
                    };
            }
            else
            {
                if (form.DaysOfWork == null)
                    employee = new Employee()
                    {
                        FullName = form.FullName,
                        Salary = form.Salary,
                        ContractBegins = form.ContractBegins,
                        ContractEnds = form.ContractEnds,
                        PictureSource = form.PictureSource,
                        User = user,
                    };
                else
                    employee = new Employee()
                    {
                        FullName = form.FullName,
                        Salary = form.Salary,
                        ContractBegins = form.ContractBegins,
                        ContractEnds = form.ContractEnds,
                        DaysOfWork = form.DaysOfWork,
                        PictureSource = form.PictureSource,
                        User = user,
                    };
            }
            //succeeded
            var result2 = await _context.Employees.AddAsync(employee);
            var result3 = await _context.SaveChangesAsync();
            await _userManager.AddToRoleAsync(user, "Employee");
            return new GenericResponse<IEnumerable<IdentityError>>()
            {
                IsSuccess = true,
                Message = "Employee Created Successfully",
            };
        }
        public async Task<GenericResponse<Employee>> UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null)
                return new GenericResponse<Employee>()
                {
                    IsSuccess = false,
                    Message = "Customer Is null"
                };
            try
            {
                var result = _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return new GenericResponse<Employee>()
                {
                    IsSuccess = true,
                    Message = "Customer Updated Successfully",
                    Result = employee
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Employee>()
                {
                    IsSuccess = false,
                    Message = "failed To Update the user",
                    Errors = new[] { ex.Message.ToString() }
                };
            }
        }

        public async Task<GenericResponse<Employee>> GetEmployeeByIdAsync(int id)
        {
            if (id == null)
                return new GenericResponse<Employee>()
                {
                    IsSuccess = false,
                    Message = "Id is null"
                };
            var result = await _context.Employees.Include(e=>e.User).FirstOrDefaultAsync(e=>e.Id == id);
            if (result == null)
                return new GenericResponse<Employee>()
                {
                    IsSuccess = false,
                    Message = "Employee Doesnt exist"
                };
            return new GenericResponse<Employee>()
            {
                IsSuccess = true,
                Message = "Employee fetched successfully",
                Result = result
            };
        }
        public async Task<GenericResponse<string>> DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
                return new GenericResponse<string>()
                {
                    IsSuccess = false,
                    Message = "Customer Is null"
                };
            var user = await _userManager.FindByIdAsync(employee.UserId);
            try
            {
                var result = _context.Employees.Remove(employee);
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
