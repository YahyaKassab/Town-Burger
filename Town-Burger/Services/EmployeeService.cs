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

        //add from the parent table not from the child indepenedently 

        Task<GenericResponse<Employee>> GetEmployeeByIdAsync(int id);
        Task<GenericResponse<IEnumerable<Employee>>> GetAllEmployees();
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

            //form isnt null
            //passwords match

            //my employee
            Employee employee = new Employee()
            {
                FullName = form.FullName,
                Salary = form.Salary,
                ContractBegins = form.ContractBegins,
                ContractEnds = form.ContractEnds,
            };
            if (form.DaysOfWork != null)
                employee.DaysOfWork = form.DaysOfWork;
            if(form.PictureSource != null)
                employee.PictureSource = form.PictureSource;

            //create the user along with the employee
            User user = new User()
            {
                Email= form.Email,
                UserName = form.Email,
                PhoneNumber= form.PhoneNumber,
                Employee = employee
            };
            

            var result = await _userManager.CreateAsync(user);
            //succeeded
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
                _context.Update(employee);
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
                return new GenericResponse<string>
                {
                    IsSuccess = false,
                    Message = "Employee Not found"
                };


            var user = await _userManager.FindByIdAsync(employee.UserId);

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
                    Message = "failed To Delete the Employee",
                    Errors = new[] { ex.Message.ToString() }
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<Employee>>> GetAllEmployees()
        {
            var employees = await _context.Employees.Include(e=>e.User).ToListAsync();
            if(employees.Count == 0)
                return new GenericResponse<IEnumerable<Employee>> { IsSuccess = false, Message = "NO employees found" };
            return new GenericResponse<IEnumerable<Employee>>
            {
                IsSuccess = true,
                Message = "All Employees Fetched",
                Result = employees
            };
        }
    }
}
