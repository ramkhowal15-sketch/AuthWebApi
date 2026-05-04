using AuthWebApi.AuthServices.AuthServe;
using AuthWebApi.Domains;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace AuthWebApi.Services.EmailServices
{
    public class Mailservices : IMailServices
    {
        private readonly IConfiguration _configuration;
        private readonly IOtpServices _otpService;

        public Mailservices(IConfiguration configuration, IOtpServices otpService)
        {
            _configuration = configuration;
            _otpService = otpService;
        }

        public async Task<string> SendMail(Mail mail)
        {
            var from = _configuration["EmailSettings:Email"];
            var password = _configuration["EmailSettings:Password"];
            var port = _configuration.GetValue<int>("EmailSettings:Port");
            var host = _configuration["EmailSettings:Host"];

            var fromMail = new MailboxAddress("Ram", from);

            var message = new MimeMessage();
            message.From.Add(fromMail);

            message.To.Add(MailboxAddress.Parse(mail.To));


            message.Subject = mail.Subject;

            message.Body = new TextPart("html")
            {
                Text = mail.Body
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(from, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

           return "Email sent successfully";
        }

    }
}
