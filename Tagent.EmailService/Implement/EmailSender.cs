using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using Tagent.EmailService.Define;
using Tagent.LoggerService;

namespace Tagent.EmailService.Implement
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly ILoggerManager _logger;
        public EmailSender(EmailConfiguration emailConfiguration, ILoggerManager logger)
        {
            _emailConfig = emailConfiguration;
            _logger = logger;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);

            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using(var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.Username, _emailConfig.Password);

                    client.Send(mailMessage);
                    _logger.LogInfo("Test logger");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message); 
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
