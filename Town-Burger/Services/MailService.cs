using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Text;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IMailService
    {
       // Task<GenericResponse<string>> SendEmailAsync(SendEmailDto model);
        //Task<GenericResponse<string>> SendViaFluentEmail(SendEmailDto model);
        Task<bool> SendViaMailKit(SendEmailDto model);
    }
    public class MailService : IMailService
    {
        public IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
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
    }
}
