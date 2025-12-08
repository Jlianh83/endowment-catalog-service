using CatalogWebApi.DTO;
using CatalogWebApi.Models;
using Azure.Communication.Email;



namespace CatalogWebApi.Service.ServiceImplement
{
    public class EmailService : IEmailService
    {
        private readonly EmailClient _emailClient; 
        private readonly IEmailTemplateBuilder _emailTemplateBuilder;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IEmailTemplateBuilder emailTemplateBuilder, EmailClient emailClient, ILogger<EmailService> logger)
        {
            _emailClient = emailClient;
            _emailTemplateBuilder = emailTemplateBuilder;
            _logger = logger;
        }

        public async Task sendEmail(SendQuotationDTO sendQuotationDTO, byte[] file, string fileName, string clientName)
        {
            try
            {

                _logger.LogInformation("Iniciando envío de email a {To} con asunto {Subject}",
                    sendQuotationDTO.To,
                    sendQuotationDTO.Subject);

                var from = "DoNotReply@b0be9ab9-956a-4e92-a666-0e6c8b94232e.azurecomm.net";
                var html = await _emailTemplateBuilder.BuildCotizationTemplateAsync(clientName);

                var emailContent = new EmailContent(sendQuotationDTO.Subject)
                {
                    Html = html
                };

                var emailMessage = new EmailMessage(from, sendQuotationDTO.To, emailContent);

                if (file != null)
                {
                    emailMessage.Attachments.Add(
                        new EmailAttachment(
                            fileName,
                            "application/pdf",
                            new BinaryData(file)));
                    _logger.LogInformation("Adjunto agregado: {FileName}", fileName);
                }

                await _emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage);

                _logger.LogInformation("Email enviado exitosamente a {To}", sendQuotationDTO.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email a {To}", sendQuotationDTO.To);
                throw; // ⚠ Propaga el error para que Azure Insights lo capture
            }


        }
    }
}
