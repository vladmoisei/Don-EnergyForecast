using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MVCWithBlazor.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IOptions<SmtpOptions> _options;
        public SmtpEmailSender(IOptions<SmtpOptions> options)
        {
            _options = options;
        }
        public async Task SendEmailAsync(string fromAdress, string toAddress, string subjsect, string messaege)
        {
            var mailMessage = new MailMessage(fromAdress, toAddress, subjsect, messaege);
            using (var client = new SmtpClient(_options.Value.Host, _options.Value.Port)
            {
                Credentials = new NetworkCredential(_options.Value.Username, _options.Value.Password),
                EnableSsl = _options.Value.EnableSsl
            })
            {
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
