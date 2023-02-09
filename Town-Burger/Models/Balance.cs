using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Identity;
using Town_Burger.Models.Responses;
using Town_Burger.Services;

namespace Town_Burger.Models
{

    public class Balance
    {
        //public Balance(Balance balance)
        //{
        //    Id= balance.Id;
        //    MyBalance = balance.MyBalance;
        //    TotalDeposits= balance.TotalDeposits;
        //    TotalEarnings= balance.TotalEarnings;
        //    TotalSpends= balance.TotalSpends;
        //    SpendsDay= balance.SpendsDay;
        //    SpendsMonth= balance.SpendsMonth;
        //    SpendsYear= balance.SpendsYear;
        //    EarningsDay= balance.EarningsDay;
        //    EarningsMonth= balance.EarningsMonth;
        //    EarningsYear = balance.EarningsYear; 
        //    DepositsDay= balance.DepositsDay;
        //    DepositsMonth= balance.DepositsMonth;
        //    DepositsYear= balance.DepositsYear;
        //}
        public int Id { get; set; }
        public double MyBalance { get; set; } = 0;
        public double TotalSpends{ get; set; } = 0;
        public double TotalDeposits{ get; set; } = 0;
        public double TotalEarnings { get; set; } = 0;

    }
    public class Spend
    {
        public int Id { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public double Amount { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;


    }
    public class Deposit
    {
        public int Id { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public double Amount { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;


    }
}
