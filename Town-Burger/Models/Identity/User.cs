using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;
using Town_Burger.Models.Dto;

namespace Town_Burger.Models.Identity
{
    public class User : IdentityUser
    {
        //customer
        //public User(string fullName,string email,string phoneNumber,IEnumerable<Address> address = null)
        //{
        //    FullName = fullName;
        //    UserName= email;
        //    Email = email;
        //    PhoneNumber= phoneNumber;
        //    if(address != null)
        //        Addresses = address;
        //}
        ////employee
        //public User(string fullName, string email, string phoneNumber, Int64? salary,DateTime? contractBegins,DateTime? contractEnds,string daysOfWork = null, byte[] picture = null)
        //{
        //    FullName = fullName;
        //    Email = email;
        //    UserName = email;
        //    PhoneNumber = phoneNumber;
        //    Salary = salary;
        //    ContractBegins= contractBegins;
        //    ContractEnds= contractEnds;
        //    if(daysOfWork!= null)
        //        DaysOfWork = daysOfWork;
        //    if(picture!= null)
        //        Picture = picture;
        //}
        public User()
        {

        }
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        //Customer
        public Cart? Cart{ get; set; }
        public IEnumerable<Order>? Orders { get;set; }
        public IEnumerable<Address>? Addresses { get; set; }
        public IEnumerable<Deposit>? DepositsCustomer { get; set; }

        //Employee
        public Int64? Salary { get; set; }
        public DateTime? ContractBegins { get; set; }
        public DateTime? ContractEnds { get; set; }
        public string? DaysOfWork { get; set; }
        public IEnumerable<Spend>? SpendsEmployee { get; set; }
        public byte[]? Picture { get; set; }
    }
}
