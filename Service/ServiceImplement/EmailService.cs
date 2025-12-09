using CatalogWebApi.DTO;
using CatalogWebApi.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace CatalogWebApi.Service.ServiceImplement
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtp;
        private readonly IEmailTemplateBuilder _emailTemplateBuilder;

        public EmailService(IOptions<SmtpSettings> smtp, IEmailTemplateBuilder emailTemplateBuilder)
        {
            _smtp = smtp.Value;
            _emailTemplateBuilder = emailTemplateBuilder;
        }

        public async Task sendEmail(SendQuotationDTO sendQuotationDTO, byte[] file, string fileName, string clientName)
        {
            try
            {
                using var client = new SmtpClient();

                await client.ConnectAsync(_smtp.Host, int.Parse(_smtp.Port), SecureSocketOptions.StartTls);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                var message = new MimeMessage();
                var html = await _emailTemplateBuilder.BuildCotizationTemplateAsync(clientName);

                message.From.Add(new MailboxAddress("From: ", _smtp.User));
                message.To.Add(new MailboxAddress("To: ", sendQuotationDTO.To));
                message.Subject = sendQuotationDTO.Subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = html
                };

                builder.Attachments.Add(fileName, file, new ContentType("application", "pdf"));

                message.Body = builder.ToMessageBody();
                await client.AuthenticateAsync(_smtp.User, _smtp.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
            }


        }
    }
}