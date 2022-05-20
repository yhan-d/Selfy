using Selfy.Core.Emails;

namespace Selfy.Business.Services
{
    public interface IEmailService
    {
        Task SendMailAsync(MailModel model);
    }
}
