using Microsoft.EntityFrameworkCore;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IBalanceService
    {
        Task<GenericResponse<double>> AddDepositAsync(string from,double amount,DateTime time);
        Task<GenericResponse<double>> AddSpendAsync(string from,double amount,DateTime time);
        Task<GenericResponse<double>> GetBalance();
        Task<GenericResponse<double>> CalculateEarnings();
    }
    public class BalanceService:IBalanceService
    {
        private readonly AppDbContext _context;

        public BalanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResponse<double>> AddDepositAsync(string from, double amount, DateTime time)
        {
            return null;
        }
        public async Task<GenericResponse<double>> AddSpendAsync(string from, double amount, DateTime time)
        {
            return null;
        }

        public async Task<GenericResponse<double>> GetBalance()
        {
            try
            {
                var balance = await _context.Balances.FirstOrDefaultAsync();
                double _balance = balance.MyBalance;
                return new GenericResponse<double> { 
                IsSuccess= true,
                Message = "Balance fetched",
                Result= _balance,
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<double>
                {
                    IsSuccess = false,
                    Message = "error fetching the balance",
                };
            }
        }

        public Task<GenericResponse<double>> CalculateEarnings()
        {
            throw new NotImplementedException();
        }
    }
}
