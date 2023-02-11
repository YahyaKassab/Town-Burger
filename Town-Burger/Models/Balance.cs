using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Identity;
using Town_Burger.Models.Responses;
using Town_Burger.Services;

namespace Town_Burger.Models
{

    public class Balance
    {
        public int Id { get; set; }
        public double MyBalance { get; set; } = 0;
        public double TotalSpends{ get; set; } = 0;
        public double TotalDeposits{ get; set; } = 0;
        public double TotalEarnings { get; set; } = 0;

    }
    public class Spend
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [Required]
        public Employee Employee{ get; set; }
        [Required]
        public double Amount { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;


    }
    public class Deposit
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [Required]
        public Customer Customer { get; set; }
        [Required]
        public double Amount { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;


    }
}
