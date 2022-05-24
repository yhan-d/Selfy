using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using Selfy.Core.Configurations;
using Selfy.Core.Emails;

namespace Selfy.Business.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            var conf = _configuration.GetSection("GmailSettings");
            this.EmailSettings = new EmailSettings
            {
                SenderMail = conf["SenderMail"],
                Password = conf["Password"],
                Smtp = conf["Smtp"],
                SmtpPort = Convert.ToInt32(conf["SmtpPort"])
            };
            //this.EmailSettings = _configuration.GetSection("GmailSettings").Get<EmailSettings>();
        }
        public EmailSettings EmailSettings { get; }

        public Task SendMailAsync(MailModel model)
        {
            var mail = new MailMessage { From = new MailAddress(this.EmailSettings.SenderMail) };

            foreach (var c in model.To)
            {
                mail.To.Add(new MailAddress(c.Adress, c.Name));
            }

            foreach (var cc in model.Cc)
            {
                mail.CC.Add(new MailAddress(cc.Adress, cc.Name));
            }

            foreach (var cc in model.Bcc)
            {
                mail.Bcc.Add(new MailAddress(cc.Adress, cc.Name));
            }

            if (model.Attachs is { Count: > 0 })
            {
                foreach (var attach in model.Attachs)
                {
                    var fileStream = attach as FileStream;
                    var info = new FileInfo(fileStream.Name);

                    mail.Attachments.Add(new Attachment(attach, info.Name));
                }
            }

            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.HeadersEncoding = Encoding.UTF8;

            mail.Subject = model.Subject;
            mail.Body = model.Body;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var smtpClient = new SmtpClient(this.EmailSettings.Smtp, this.EmailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(this.EmailSettings.SenderMail, this.EmailSettings.Password),
                EnableSsl = true
            };
            return smtpClient.SendMailAsync(mail);
        }
    }
}
