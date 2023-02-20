using System.ComponentModel.DataAnnotations;

namespace Town_Burger.Models.Dto
{
    public class ReturnedEmployee
    {

        public int Id { get; set; }
        public string FullName { get; set; }
        public Int64 Salary { get; set; }
        public DateTime? ContractBegins { get; set; }
        public DateTime? ContractEnds { get; set; }
        public string? DaysOfWork { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string? Token { get; set; }
    }
}
