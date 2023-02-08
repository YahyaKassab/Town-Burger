using Town_Burger.Models;
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
        public async Task<GenericResponse<double>> AddDepositAsync(string from, double amount, DateTime time)
        {
            return null;
        }
        public async Task<GenericResponse<double>> AddSpendAsync(string from, double amount, DateTime time)
        {
            return null;
        }

        public Task<GenericResponse<double>> GetBalance()
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<double>> CalculateEarnings()
        {
            throw new NotImplementedException();
        }
    }
}
