using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using EmailServices.EmailDomain;
using EmailServices.Interfaces;

namespace EmailServices
{
    public class EmailService:IEmailService
    {
        private SmtpClient _smtpServer;

        public EmailService(string smtpHostServer)
        {
            _smtpServer = new SmtpClient(smtpHostServer);
        }
        public string EmailSmtpService{get;set;}
        public void SendEmail(TicketMasterEmailMessage message)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["BusinessEmail"]);
            mailMessage.Subject = string.Format("Message From: {0}, {1}",mailMessage.From, mailMessage.Subject);
            foreach(var to in message.EmailTo)
            {
                mailMessage.To.Add(new MailAddress(to));
            }
            if (!string.IsNullOrEmpty(message.AttachmentFilePath))
                mailMessage.Attachments.Add(new Attachment(message.AttachmentFilePath));
            else if(message.AttachmentStream != null)
            {
                mailMessage.Attachments.Add(new Attachment(message.AttachmentStream, message.AttachedFileName));
            }

            mailMessage.Subject = message.Subject;
            mailMessage.Body = message.EmailMessage;
            _smtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["BusinessEmail"], ConfigurationManager.AppSettings["SmtpPassword"]);

            _smtpServer.Send(mailMessage);
        }
    }
}
