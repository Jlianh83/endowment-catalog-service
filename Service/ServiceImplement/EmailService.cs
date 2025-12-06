using CatalogWebApi.DTO;
using CatalogWebApi.Models;
using Azure.Communication.Email;



namespace CatalogWebApi.Service.ServiceImplement
{
    public class EmailService : IEmailService
    {
        private readonly EmailClient _emailClient; 
        private readonly IEmailTemplateBuilder _emailTemplateBuilder;

        public EmailService(IEmailTemplateBuilder emailTemplateBuilder, IConfiguration configuration)
        {
           _emailClient = new EmailClient(configuration["ConnectionStrings:AzureCommunication"]);
            _emailTemplateBuilder = emailTemplateBuilder;
        }

        public async Task sendEmail(SendQuotationDTO sendQuotationDTO, byte[] file, string fileName, string clientName)
        {
            try
            {

                var from = "DoNotReply@24291be4-f291-46a9-a4a8-c70599016ee0.azurecomm.net";
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
                }

                await _emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage); 
               
            } catch (Exception ex) 
            {
                Console.WriteLine($"Error:{ex.Message}");
            }
            

        }
    }
}
