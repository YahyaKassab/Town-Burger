using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Identity;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IBalanceService
    {
        Task<GenericResponse<double>> AddDepositAsync(int fromId, double amount);
        Task<GenericResponse<double>> AddSpendAsync(int fromId, double amount);
        Task<GenericResponse<double>> AddToBalanceAsync(double amount);
        Task<GenericResponse<double>> SubFromBalanceAsync(double amount);
        Task<GenericResponse<Balance>> GetBalance();
        Task<GenericResponse<double>> GetEarningsDay();
        Task<GenericResponse<IEnumerable<Deposit>>> GetDepositsDay();
        Task<GenericResponse<IEnumerable<Spend>>> GetSpendsDay();
        Task<GenericResponse<double>> GetEarningsMonth();
        Task<GenericResponse<IEnumerable<Deposit>>> GetDepositsMonth();
        Task<GenericResponse<IEnumerable<Spend>>> GetSpendsMonth();
        Task<GenericResponse<double>> GetEarningsYear();
        Task<GenericResponse<IEnumerable<Deposit>>> GetDepositsYear();
        Task<GenericResponse<IEnumerable<Spend>>> GetSpendsYear();
    }
    public class BalanceService:IBalanceService
    {
        private readonly AppDbContext _context;
        private UserManager<User> _userManager;
        private readonly ICustomerService _cutomerService;
        private readonly IEmployeeService _employeeService;

        public BalanceService(AppDbContext context, UserManager<User> userManager, ICustomerService cutomerService, IEmployeeService employeeService)
        {
            _context = context;
            _userManager = userManager;
            _cutomerService = cutomerService;
            _employeeService = employeeService;
        }

        public async Task<GenericResponse<double>> AddDepositAsync(int fromId, double amount)
        {
            var customer = await _cutomerService.GetCustomerByIdAsync(fromId);
            var balance = await GetBalance();
            var _balance = balance.Result;

            //customer Doesnt exist
            if (!customer.IsSuccess) return new GenericResponse<double>
            {
                IsSuccess = false,
                Message = "No customer With this Id"
            };
            //customer exists and is customer
            var deposit = new Deposit()
            {
                CustomerId = fromId,
                Amount = amount,
                Time = DateTime.Now,
            };

            var result = await _context.Deposits.AddAsync(deposit);
            _balance.TotalDeposits += amount;
            _balance.TotalEarnings += amount;
            _context.Balances.Update(_balance);
            await _context.SaveChangesAsync();
            await AddToBalanceAsync(amount);
            return new GenericResponse<double>
            {
                IsSuccess = true,
                Message = "Deposit Added Successfully",
                Result = amount
            };


        }
        public async Task<GenericResponse<double>> AddSpendAsync(int fromId, double amount)
        {

            var employee = await _employeeService.GetEmployeeByIdAsync(fromId);
            var balance = await GetBalance();
            var _balance = balance.Result;

            //customer Doesnt exist
            if (!employee.IsSuccess) return new GenericResponse<double>
            {
                IsSuccess = false,
                Message = "No employee With this Id"
            };
            //customer exists and is employee
                var spend = new Spend()
                {
                    EmployeeId= fromId,
                    Amount = amount,
                    Time = DateTime.Now
                };

            var result = await _context.AddAsync(spend);
            _balance.TotalSpends += amount;
            _balance.TotalEarnings -= amount;
            await _context.SaveChangesAsync();
            await SubFromBalanceAsync(amount);
            return new GenericResponse<double>
            {
                IsSuccess = true,
                Message = "Spend Added Successfully",
                Result = amount
            };


        }

        public async Task<GenericResponse<Balance>> GetBalance()
        {
            try
            {
                var balance = await _context.Balances.FirstOrDefaultAsync();
                return new GenericResponse<Balance> { 
                IsSuccess= true,
                Message = "Balance fetched",
                Result= balance,
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<Balance>
                {
                    IsSuccess = false,
                    Message = "error fetching the balance",
                };
            }
        }

        public async Task<GenericResponse<double>> AddToBalanceAsync(double amount)
        {
            var balance = await GetBalance();
            if(!balance.IsSuccess)
                return new GenericResponse<double> { 
                    IsSuccess = false,
                    Message = "Failed Getting The balance"
                };

            try
            {
                var newBalance = balance.Result;
                newBalance.MyBalance += amount;
                var result = _context.Balances.Update(newBalance);
                await _context.SaveChangesAsync();
                return new GenericResponse<double>
                {
                    IsSuccess = true,
                    Message = "Amount Added Successfully",
                    Result = newBalance.MyBalance
                };
            }catch (Exception ex)
            {
                return new GenericResponse<double>
                {
                    IsSuccess = false,
                    Message = "Adding failed",
                };
            }

        }

        public async Task<GenericResponse<double>> SubFromBalanceAsync(double amount)
        {

            var balance = await GetBalance();
            if (!balance.IsSuccess)
                return new GenericResponse<double>
                {
                    IsSuccess = false,
                    Message = "Failed Getting The balance"
                };

            try
            {
                var newBalance = balance.Result;
                newBalance.MyBalance -= amount;
                var result = _context.Balances.Update(newBalance);
                await _context.SaveChangesAsync();
                return new GenericResponse<double>
                {
                    IsSuccess = true,
                    Message = "Amount Added Successfully",
                    Result = newBalance.MyBalance
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<double>
                {
                    IsSuccess = false,
                    Message = "Adding failed",
                };
            }

        }

        public async Task<GenericResponse<double>> GetEarningsDay()
        {
            var depositsObject = GetDepositsDay();
            var spendsObject = GetSpendsDay();
            var deposits = depositsObject.Result.Result.ToList();
            var spends = spendsObject.Result.Result.ToList();

            double totalDeposits = 0;
            double totalSpends = 0;
            foreach ( var deposit in deposits)
                totalDeposits += deposit.Amount;
            foreach ( var spend in spends)
                totalSpends += spend.Amount;
            double earnings = totalDeposits - totalSpends;
            return new GenericResponse<double>
            {
                IsSuccess = true,
                Message = "Earnings of the day Calculated",
                Result = earnings
            };
        }

        public async Task<GenericResponse<IEnumerable<Deposit>>> GetDepositsDay()
        {
            var deposits = _context.Deposits.Where(e => e.Time > DateTime.Now.AddDays(-1));
            return new GenericResponse<IEnumerable<Deposit>>
            {
                IsSuccess = true,
                Message = "Deposits in the last 24h are fetched",
                Result = deposits.ToList()
            };
        }

        public async Task<GenericResponse<IEnumerable<Spend>>> GetSpendsDay()
        {
            var spends = _context.Spends.Where(e => e.Time > DateTime.Now.AddDays(-1));
            return new GenericResponse<IEnumerable<Spend>>
            {
                IsSuccess = true,
                Message = "Spends in the last 24h are fetched",
                Result = spends.ToList()
            };
        }

        public async Task<GenericResponse<double>> GetEarningsMonth()
        {
            var depositsObject = GetDepositsMonth();
            var spendsObject = GetSpendsMonth();
            var deposits = depositsObject.Result.Result.ToList();
            var spends = spendsObject.Result.Result.ToList();

            double totalDeposits = 0;
            double totalSpends = 0;
            foreach (var deposit in deposits)
                totalDeposits += deposit.Amount;
            foreach (var spend in spends)
                totalSpends += spend.Amount;
            double earnings = totalDeposits - totalSpends;
            return new GenericResponse<double>
            {
                IsSuccess = true,
                Message = "Earnings of the Month Calculated",
                Result = earnings
            };
        }

        public async Task<GenericResponse<IEnumerable<Deposit>>> GetDepositsMonth()
        {
            var deposits = _context.Deposits.Where(e => e.Time > DateTime.Now.AddMonths(-1));
            return new GenericResponse<IEnumerable<Deposit>>
            {
                IsSuccess = true,
                Message = "Deposits in the last Month are fetched",
                Result = deposits.ToList()
            };
        }

        public async  Task<GenericResponse<IEnumerable<Spend>>> GetSpendsMonth()
        {
            var spends = _context.Spends.Where(e => e.Time > DateTime.Now.AddMonths(-1));
            return new GenericResponse<IEnumerable<Spend>>
            {
                IsSuccess = true,
                Message = "Spends in the last Month are fetched",
                Result = spends.ToList()
            };
        }

        public async Task<GenericResponse<double>> GetEarningsYear()
        {
            var depositsObject = GetDepositsYear();
            var spendsObject = GetSpendsYear();
            var deposits = depositsObject.Result.Result.ToList();
            var spends = spendsObject.Result.Result.ToList();

            double totalDeposits = 0;
            double totalSpends = 0;
            foreach (var deposit in deposits)
                totalDeposits += deposit.Amount;
            foreach (var spend in spends)
                totalSpends += spend.Amount;
            double earnings = totalDeposits - totalSpends;
            return new GenericResponse<double>
            {
                IsSuccess = true,
                Message = "Earnings of the Month Calculated",
                Result = earnings
            };
        }

        public async Task<GenericResponse<IEnumerable<Deposit>>> GetDepositsYear()
        {
            var deposits = _context.Deposits.Where(e => e.Time > DateTime.Now.AddYears(-1));
            return new GenericResponse<IEnumerable<Deposit>>
            {
                IsSuccess = true,
                Message = "Deposits in the last Month are fetched",
                Result = deposits.ToList()
            };
        }

        public async Task<GenericResponse<IEnumerable<Spend>>> GetSpendsYear()
        {
            var spends = _context.Spends.Where(e => e.Time > DateTime.Now.AddYears(-1));
            return new GenericResponse<IEnumerable<Spend>>
            {
                IsSuccess = true,
                Message = "Spends in the last Year are fetched",
                Result = spends.ToList()
            };
        }
    }
}
