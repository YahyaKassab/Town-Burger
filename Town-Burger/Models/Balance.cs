using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Responses;
using Town_Burger.Services;

namespace Town_Burger.Models
{

    public class Balance
    {
        public readonly IBalanceService _balanceService;
        public static double _balance{ get; set; }
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
