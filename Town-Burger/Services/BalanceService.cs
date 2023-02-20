using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Identity;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IBalanceService
    {

        //add from the parent table not from the child indepenedently 

        Task<GenericResponse<double>> AddDepositAsync(int fromId, double amount);
        Task<GenericResponse<double>> AddSpendAsync(int fromId, double amount);
        //Task<GenericResponse<Spend>> EditSpend(Spend spend);
        //Task<GenericResponse<Deposit>> EditDeposit(Deposit deposit);
        Task<GenericResponse<double>> AddToBalanceAsync(double amount);
        Task<GenericResponse<double>> SubFromBalanceAsync(double amount);
        Task<GenericResponse<Balance>> GetBalance();
        Task<GenericResponse<double>> GetEarningsDay();
        Task<GenericResponse<IEnumerable<Deposit>>> GetDepositsDay();
        Task<GenericResponse<IEnumerable<Spend>>> GetSpendsDay();
        Task<GenericResponse<double>> GetEarningsMonth();
        Task<GenericResponse<IEnumerable<ReturnedDeposit>>> GetDepositsMonth();
        Task<GenericResponse<IEnumerable<Spend>>> GetSpendsMonth();
        Task<GenericResponse<double>> GetEarningsYear();
        Task<GenericResponse<IEnumerable<Deposit>>> GetDepositsYear();
        Task<GenericResponse<IEnumerable<Deposit>>> GetDepositsTotal();
        Task<GenericResponse<IEnumerable<Spend>>> GetSpendsYear();
        Task<GenericResponse<IEnumerable<Spend>>> GetSpendsTotal();
        Task<GenericResponse<double>> GetEarningsTotal();
        Task<GenericResponse<Spend>> GetSpendById(int id);
        Task<GenericResponse<Deposit>> GetDepositById(int id);
        Task<GenericResponse<Deposit>> DeleteDeposit(int id);
        Task<GenericResponse<Spend>> DeleteSpend(int id);
    }
    public class BalanceService:IBalanceService
    {
        private readonly AppDbContext _context;

        public BalanceService(AppDbContext context)
        {
            _context = context;

        }

        public async Task<GenericResponse<double>> AddDepositAsync(int fromId, double amount)
        {
            var customer = await _context.Customers.Include(c=>c.DepositsCustomer).SingleOrDefaultAsync(c=>c.Id == fromId);
            var balance = await GetBalance();
            var _balance = balance.Result;

            //customer Doesnt exist
            if (customer == null) return new GenericResponse<double>
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

            //customer.DepositsCustomer.Add(deposit);
            await _context.AddAsync(deposit);
            _balance.TotalDeposits += amount;
            _balance.TotalEarnings += amount;
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

            var employee = await _context.Employees.Include(e=>e.SpendsEmployee).FirstOrDefaultAsync(e=>e.Id == fromId);
            var balance = await GetBalance();
            var _balance = balance.Result;

            //customer Doesnt exist
            if (employee == null) return new GenericResponse<double>
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

            employee.SpendsEmployee.Add(spend);
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
            var deposits = _context.Deposits.Include(d=>d.Customer).Where(e => e.Time > DateTime.Now.AddDays(-1));
            return new GenericResponse<IEnumerable<Deposit>>
            {
                IsSuccess = true,
                Message = "Deposits in the last 24h are fetched",
                Result = deposits.ToList()
            };
        }

        public async Task<GenericResponse<IEnumerable<Spend>>> GetSpendsDay()
        {
            var spends = _context.Spends.Include(s=>s.Employee).Where(e => e.Time > DateTime.Now.AddDays(-1));
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

        public async Task<GenericResponse<IEnumerable<ReturnedDeposit>>> GetDepositsMonth()
        {
            var deposits = _context.Deposits.Include(d=>d.Customer).ThenInclude(c=>c.User).Where(e => e.Time > DateTime.Now.AddMonths(-1));

            var depositsToReturn = new List<ReturnedDeposit>();
            
            foreach (var deposit in deposits)
            {
                depositsToReturn.Add(
                    new ReturnedDeposit
                    {
                        Amount = deposit.Amount,
                        CustomerEmail = deposit.Customer.User.Email,
                        CustomerId = deposit.CustomerId,
                        CustomerName = deposit.Customer.FullName,
                        CustomerPhone = deposit.Customer.User.PhoneNumber,
                        Id = deposit.Id,
                        Time = deposit.Time,
                    }
                    );
            }

            return new GenericResponse<IEnumerable<ReturnedDeposit>>
            {
                IsSuccess = true,
                Message = "Deposits in the last Month are fetched",
                Result = depositsToReturn
            };
        }

        public async  Task<GenericResponse<IEnumerable<Spend>>> GetSpendsMonth()
        {
            var spends = _context.Spends.Include(s=>s.Employee).Where(e => e.Time > DateTime.Now.AddMonths(-1));
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
            var deposits = _context.Deposits.Include(d=>d.Customer).Where(e => e.Time > DateTime.Now.AddYears(-1));
            return new GenericResponse<IEnumerable<Deposit>>
            {
                IsSuccess = true,
                Message = "Deposits in the last Month are fetched",
                Result = deposits.ToList()
            };
        }

        public async Task<GenericResponse<IEnumerable<Spend>>> GetSpendsYear()
        {
            var spends = _context.Spends.Include(s=>s.Employee).Where(e => e.Time > DateTime.Now.AddYears(-1));
            return new GenericResponse<IEnumerable<Spend>>
            {
                IsSuccess = true,
                Message = "Spends in the last Year are fetched",
                Result = spends.ToList()
            };
        }

        //public async Task<GenericResponse<Spend>> EditSpend(Spend spend)
        //{
        //    //10 9 new - old
        //    //9 10 new - old
        //    try
        //    {
        //        var oldSpend = await GetSpendById(spend.Id);
        //        if (!oldSpend.IsSuccess)
        //            return new GenericResponse<Spend>
        //            {
        //                IsSuccess = false,
        //                Message = "Spend doesnt exist"
        //            };
        //        double balanceToAdd = spend.Amount - oldSpend.Result.Amount;
        //        _context.Update(spend);
        //        await _context.SaveChangesAsync();
        //        AddToBalanceAsync(balanceToAdd);
        //        return new GenericResponse<Spend>
        //        { 
        //            IsSuccess = true,
        //            Message = "Updated Successfully",
        //            Result = spend 
        //        };
        //    }catch(Exception ex)
        //    {
        //        return new GenericResponse<Spend>
        //        {
        //            IsSuccess = false,
        //            Message = ex.Message,
        //        };
        //    }
        //}

        //public async Task<GenericResponse<Deposit>> EditDeposit(Deposit deposit)
        //{
        //    //10 9 new - old
        //    //9 10 new - old
        //    try
        //    {
        //        _context.Update(deposit);
        //        await _context.SaveChangesAsync();
        //        return new GenericResponse<Deposit>
        //        {
        //            IsSuccess = true,
        //            Message = "Updated Successfully",
        //            Result = deposit
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GenericResponse<Deposit>
        //        {
        //            IsSuccess = false,
        //            Message = ex.Message,
        //        };
        //    }
        //}

        public async Task<GenericResponse<IEnumerable<Deposit>>> GetDepositsTotal()
        {
            try
            {
                var deposits = await _context.Deposits.Include(d => d.Customer).ToListAsync();
                if (deposits.Count == 0)
                    return new GenericResponse<IEnumerable<Deposit>>
                    {
                        IsSuccess = true,
                        Message = "No Deposits Yet"
                    };
                return new GenericResponse<IEnumerable<Deposit>>
                {
                    IsSuccess = true,
                    Message = "Deposits fetched successfully",
                    Result = deposits
                };
            }catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<Deposit>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<Spend>>> GetSpendsTotal()
        {
            try
            {
                var spends = await _context.Spends.Include(d => d.Employee).ToListAsync();
                if (spends.Count == 0)
                    return new GenericResponse<IEnumerable<Spend>>
                    {
                        IsSuccess = true,
                        Message = "No Spends Yet"
                    };
                return new GenericResponse<IEnumerable<Spend>>
                {
                    IsSuccess = true,
                    Message = "Spends fetched successfully",
                    Result = spends
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<Spend>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<GenericResponse<double>> GetEarningsTotal()
        {
            var balance = await GetBalance();
            if (!balance.IsSuccess)
                return new GenericResponse<double> { IsSuccess = false, Message = "balance not found" };
            return new GenericResponse<double>
            {
                IsSuccess = true,
                Message = "Total Earnings Calculated",
                Result = balance.Result.TotalEarnings
            };
        }

        public async Task<GenericResponse<Spend>> GetSpendById(int id)
        {
            try
            {
                var spend = await _context.Spends.Include(s => s.Employee).FirstOrDefaultAsync(s => s.Id == id);
                if (spend == null)
                    return new GenericResponse<Spend>
                    {
                        IsSuccess = false,
                        Message = "No Spend with this id",
                    };
                return new GenericResponse<Spend>
                {
                    IsSuccess = true,
                    Message = "Spend fetched successfully",
                    Result = spend
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Spend>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<GenericResponse<Deposit>> GetDepositById(int id)
        {
            try
            {
                var deposit = await _context.Deposits.Include(s => s.Customer).FirstOrDefaultAsync(s => s.Id == id);
                if (deposit == null)
                    return new GenericResponse<Deposit>
                    {
                        IsSuccess = false,
                        Message = "No Deposit with this id",
                    };
                return new GenericResponse<Deposit>
                {
                    IsSuccess = true,
                    Message = "Deposit fetched successfully",
                    Result = deposit
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Deposit>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<GenericResponse<Deposit>> DeleteDeposit(int id)
        {
            try
            {
                var deposit = await GetDepositById(id);
                var balance = await GetBalance();
                if (!balance.IsSuccess)
                    return new GenericResponse<Deposit> { IsSuccess = false, Message = "Balance not found" };

                if (!deposit.IsSuccess)
                    return new GenericResponse<Deposit>
                    {
                        IsSuccess = false,
                        Message = "No Deposit found"
                    };
                double amount = deposit.Result.Amount;
                _context.Remove(deposit.Result);
                await _context.SaveChangesAsync();
                await SubFromBalanceAsync(amount);
                balance.Result.TotalDeposits -= amount;
                balance.Result.TotalEarnings -= amount;
                await _context.SaveChangesAsync();
                return new GenericResponse<Deposit>
                {
                    IsSuccess = true,
                    Message = "Deposit Deleted Successfully",
                    Result = deposit.Result
                };
            }catch (Exception ex)
            {
                return new GenericResponse<Deposit>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<GenericResponse<Spend>> DeleteSpend(int id)
        {
            try
            {
                var spend = await GetSpendById(id);
                var balance = await GetBalance();
                if (!balance.IsSuccess)
                    return new GenericResponse<Spend> { IsSuccess = false, Message = "Balance not found" };
                if (!spend.IsSuccess)
                    return new GenericResponse<Spend>
                    {
                        IsSuccess = false,
                        Message = "No Deposit found"
                    };
                double amount = spend.Result.Amount;
                _context.Remove(spend.Result);
                await _context.SaveChangesAsync();
                await AddToBalanceAsync(amount);
                balance.Result.TotalSpends -= amount;
                balance.Result.TotalEarnings += amount;
                await _context.SaveChangesAsync();
                return new GenericResponse<Spend>
                {
                    IsSuccess = true,
                    Message = "Spend Deleted Successfully",
                    Result = spend.Result
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Spend>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

    }
}
