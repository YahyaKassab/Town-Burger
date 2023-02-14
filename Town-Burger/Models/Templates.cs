namespace Town_Burger.Models
{
    public class ConfirmEmail
    {

        public ConfirmEmail(string confirmEmailLink)
        {
            Template = $"<h1 style='text-align: center; margin-top: 10px; color: brown'>Town Burger</h1>\r\n<h2 style='text-align: center; margin-top: 10px'>\r\n  Thank you for signing up in our account\r\n</h2>\r\n<br />\r\n<br />\r\n<h3 style='text-align: center; margin-top: 10px'>\r\n  Please confirm Your account by clicking\r\n  <a\r\n    style='\r\n      color: white;\r\n      padding-left: 20px;\r\n      padding-right: 20px;\r\n      padding-bottom: 15px;\r\n      padding-top: 15px;\r\n      margin-top: 20px;\r\n      background-color: brown;\r\n      border-radius: 15px;\r\n      text-decoration: none;\r\n    '\r\n    href='{confirmEmailLink}'\r\n    >Here</a\r\n  >\r\n</h3>\r\n<br />\r\n<br />\r\n<br />\r\n<h2 style='text-align: center; margin-top: 10px'>\r\n  If you didnt create an account for Town Burger Recently, Just Ignore This\r\n  Email\r\n</h2>\r\n";
        }
        public string Template { get; set; }

    }
    public class ForgetPassord
    {

        public ForgetPassord(string resetPasswordLink)
        {
            Template = $"<h1 style='text-align: center; margin-top: 10px; color: brown'>Town Burger</h1>\r\n<br />\r\n<h2 style='text-align: center'>\r\n  We recieved your <strong>RESET PASSWORD</strong> Request\r\n</h2>\r\n<h3 style='text-align: center'>\r\n  Now You Can Reset Your Password By Clicking on the Following Button\r\n  <br />\r\n  <br />\r\n  <br />\r\n  <a\r\n    style='\r\n      color: white;\r\n      padding-left: 20px;\r\n      padding-right: 20px;\r\n      padding-bottom: 15px;\r\n      padding-top: 15px;\r\n      margin-top: 50px;\r\n      background-color: brown;\r\n      border-radius: 15px;\r\n      text-decoration: none;\r\n    '\r\n    href='{resetPasswordLink}'\r\n    >Reset Password</a\r\n  >\r\n</h3>\r\n<br />\r\n<br />\r\n<br />\r\n<h2 style='text-align: center; margin-top: 10px'>\r\n  If you didnt request a Password reset Recently, Just Ignore This Email\r\n</h2>\r\n";
        }
        public string Template { get; set; }

    }
    public class OrderOut
    {
        public OrderOut(string customerName)
        {
            Template = $"<h1 style='text-align: center; margin-top: 10px; color: brown'>Town Burger</h1>\r\n<h2 style='text-align: center; margin-top: 10px'>\r\n  Thank you {customerName} for Ordering From Town Burger\r\n</h2>\r\n<br />\r\n<h3 style='text-align: center; margin-top: 10px'>\r\n  Your Order is Ready and it is on its way to your house\r\n</h3>\r\n<br />\r\n<h2 style='text-align: center; margin-top: 10px'>\r\n  You will be expecting a <span style='color: brown'>call</span> from the\r\n  delivery in the <span style='color: brown'>next 20 mins</span>\r\n</h2>\r\n<br />\r\n<br />\r\n";
        }
        public string Template { get; set; }

    }
    public class OrderPlaced
    {
        public OrderPlaced(string customerName)
        {
            Template = $"<h1 style='text-align: center; margin-top: 10px; color: brown'>Town Burger</h1>\r\n<h2 style='text-align: center; margin-top: 10px'>\r\n  Thank you {customerName} for Ordering From Town Burger\r\n</h2>\r\n<br />\r\n<h3 style='text-align: center; margin-top: 10px'>\r\n  Your Order is Being Prepaired Right Now\r\n</h3>\r\n<br />\r\n<h2 style='text-align: center; margin-top: 10px'>\r\n  Your Order will be arriving in the <span style='color: brown'>next 45 mins</span>\r\n</h2>\r\n<br />\r\n<br />\r\n";
        }
        public string Template { get; set; }

    }
}
