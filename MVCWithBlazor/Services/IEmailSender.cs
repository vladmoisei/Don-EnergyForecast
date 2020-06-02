using System.Threading.Tasks;

namespace MVCWithBlazor.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string fromAdress, string toAddress, string subjsect, string messaege);
    }
}
