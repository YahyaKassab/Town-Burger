using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Text;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Identity;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IMailService
    {
       // Task<GenericResponse<string>> SendEmailAsync(SendEmailDto model);
        //Task<GenericResponse<string>> SendViaFluentEmail(SendEmailDto model);
        Task<bool> SendViaMailKit(SendEmailDto model);
        Task<GenericResponse<bool>> SendConfirmEmail(string email);
        Task<GenericResponse<bool>> SendResetPasswordEmail(string email);

        Task<GenericResponse<bool>> SendOrderOutEmail(int id);
        Task<GenericResponse<bool>> SendOrderPlacedEmail(int id);

    }
    public class MailService : IMailService
    {
        private IConfiguration _configuration;
        private UserManager<User> _userManager;
        private AppDbContext _context;

        public MailService(IConfiguration configuration, UserManager<User> userManager, AppDbContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
        }

        public async Task<GenericResponse<bool>> SendConfirmEmail(string email)
        {
            try
            {
                var customer = await _userManager.FindByEmailAsync(email);
                if(customer == null)
                {
                    return new GenericResponse<bool> { IsSuccess = false, Message = "Customer Not Found" };
                }
                //Customer found
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(customer);
                //might contain special chars so we have to encode it again

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);

                //as a string

                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["BackUrl"]}/User/confirmEmail?userId={customer.Id}&token={validEmailToken}";

                var template = new ConfirmEmail(url).Template;
                var success = await SendViaMailKit(new SendEmailDto { Subject = "Confirm Your Town Burger Email", HtmlBody = template, To = customer.Email});
                return new GenericResponse<bool>
                {
                    IsSuccess= success,
                    Message = "Success i guess"
                };
            }catch(Exception ex)
            {
                return new GenericResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Sending failed"
                };
            }
        }

        public async Task<GenericResponse<bool>> SendOrderOutEmail(int id)
        {
            try
            {
                var customer = await _context.Customers.Include(c=>c.User).FirstOrDefaultAsync(c=>c.Id == id);

                if (customer == null)
                {
                    return new GenericResponse<bool> { IsSuccess = false, Message = "Customer Not Found" };
                }
                //Customer found

                var template = new OrderOut(customer.FullName).Template;
                var success = await SendViaMailKit(new SendEmailDto { Subject = "Your Order Is Out", HtmlBody = template, To = customer.User.Email });
                return new GenericResponse<bool>
                {
                    IsSuccess = success,
                    Message = "Success i guess"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Sending failed"
                };
            }
        }

        public async Task<GenericResponse<bool>> SendResetPasswordEmail(string Email)
        {
            try
            {
                var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.User.Email == Email);
                if (customer == null)
                {
                    return new GenericResponse<bool> { IsSuccess = false, Message = "Customer Not Found" };
                }
                //Customer found


                var token = await _userManager.GeneratePasswordResetTokenAsync(customer.User);
                var tokenEncoded = Encoding.UTF8.GetBytes(token);
                var tokenAsString = WebEncoders.Base64UrlEncode(tokenEncoded);


                //send to our controller
                string url = $"{_configuration["FrontUrl"]}/reset-Password/{customer.User.Email}/{tokenAsString}";


                var template = new ForgetPassord(url).Template;
                var success = await SendViaMailKit(new SendEmailDto { Subject = "Reset Your Password", HtmlBody = template, To = customer.User.Email });
                return new GenericResponse<bool>
                {
                    IsSuccess = success,
                    Message = "Success i guess"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Sending failed"
                };
            }
        }
        public async Task<bool> SendViaMailKit(SendEmailDto model)
        {
            try
            {
                string host = _configuration["MailSettings:Host"];
                int port = int.Parse(_configuration["MailSettings:Port"]);
                string _email = _configuration["MailSettings:Email"];
                string password = _configuration["MailSettings:Password"];
                string display = _configuration["MailSettings:DisplayName"];
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_configuration["MailSettings:Email"]),
                    Subject = model.Subject,
                };


                email.To.Add(MailboxAddress.Parse(model.To));

                var builder = new BodyBuilder();

                builder.HtmlBody = model.HtmlBody;

                email.Body = builder.ToMessageBody();

                email.From.Add(new MailboxAddress(display, _email));

                using var smtp = new SmtpClient();
                smtp.Connect(host, port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_email, password);
                await smtp.SendAsync(email);

                smtp.Disconnect(true);


                return true;
            }catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<GenericResponse<bool>> SendOrderPlacedEmail(int id)
        {

            try
            {
                var customer = await _context.Customers.Include(c=>c.User).FirstOrDefaultAsync(c=>c.Id == id);

                if (customer == null)
                {
                    return new GenericResponse<bool> { IsSuccess = false, Message = "Customer Not Found" };
                }
                //Customer found


                var template = new OrderPlaced(customer.FullName).Template;
                var success = await SendViaMailKit(new SendEmailDto { Subject = "Your Order Was Placed", HtmlBody = template, To = customer.User.Email });
                return new GenericResponse<bool>
                {
                    IsSuccess = success,
                    Message = "Success i guess"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Sending failed"
                };
            }
        }

        //public async Task<GenericResponse<string>> SendEmailAsync(SendEmailDto model)
        //{
        //    try
        //    {
        //        string password = _configuration["Mail:Password"];
        //        MailMessage message = new MailMessage();
        //        message.From = new MailAddress(model.From);
        //        message.Subject = model.Subject;
        //        message.To.Add(new MailAddress(model.To));
        //        message.Body = model.HtmlBody;
        //        message.IsBodyHtml = true;

        //        var smtpClient = new SmtpClient("smtp@gmail.com")
        //        {
        //            Port = 587,
        //            Credentials = new NetworkCredential(model.From, password),
        //            EnableSsl = true
        //        };
        //        smtpClient.SendMailAsync(message);
        //        return new GenericResponse<MailMessage>
        //        {
        //            IsSuccess = true,
        //            Message = "Mail sent Successfully",
        //            Result = message
        //        };
        //    }catch(Exception ex)
        //    {
        //        return new GenericResponse<MailMessage>
        //        {
        //            IsSuccess = false,
        //            Message = ex.Message
        //        };
        //    }

        //}

        //public async Task<GenericResponse<string>> SendViaFluentEmail(SendEmailDto model)
        //{


        //    //Connect to a Gmail server
        //    var sender = new SmtpSender(() => new SmtpClient(_configuration["Mail:FluentHost"])
        //    {
        //        EnableSsl = false,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        Port = 25
        //    });



        //    //Our Template that require FluentEmail.Razor
        //    StringBuilder template = new();
        //    template.AppendLine("Dear @Model.FirstName,");
        //    template.AppendLine("<p>Thanks for signing in</p>");



        //    Email.DefaultSender = sender;
        //    Email.DefaultRenderer = new RazorRenderer();




        //    var email = await Email
        //        //my claim but it is going to send from my email anyways
        //        .From(model.From)
        //        .To(model.To, "Name")
        //        .Subject(model.Subject)
        //        .UsingTemplate(template.ToString(), new {FirstName = "Nigga"})
        //      //  .Body(model.HtmlBody)
        //        .SendAsync();


        //    if (email.Successful)
        //        return new GenericResponse<string>
        //        {
        //            IsSuccess = true,
        //            Message = "Email Sent Successfully",
        //        };


        //    return new GenericResponse<string>
        //    {
        //        IsSuccess = false,
        //        Message = "Failed"
        //    };
        //}

    }
}
