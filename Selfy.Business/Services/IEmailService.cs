namespace Selfy.Business.Services
{
    public interface IEmailService
    {
        Task SendMailAsync(MailModel model);
    }
}
