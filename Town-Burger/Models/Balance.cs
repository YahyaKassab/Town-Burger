using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Responses;
using Town_Burger.Services;

namespace Town_Burger.Models
{

    public class Balance
    {
        public int Id { get; set; }
        public double MyBalance { get; set; } = 0;
        public double TotalSpends{ get; set; } = 0;
        public double SpendsDay{ get; set; } = 0;
        public double SpendsMonth{ get; set; } = 0;
        public double SpendsYear{ get; set; } = 0;
        public double TotalDeposits{ get; set; } = 0;
        public double DepositsDay { get; set; } = 0;
        public double DepositsMonth { get; set; } = 0;
        public double DepositsYear { get; set; } = 0;
        
        public double TotalEarnings { get; set; } = 0;
        public double EarningsDay { get; set; } = 0;
        public double EarningsMonth { get; set; } = 0;
        public double EarningsYear { get; set; } = 0;

    }
    public class Spend<T>
    {
        public int Id { get; set; }
        [Required]
        public T Where { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public DateTime Time { get; set; }


    }
    public class Deposit<T>
    {
        public int Id { get; set; }
        [Required]
        public T Where { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public DateTime Time { get; set; }


    }
}
