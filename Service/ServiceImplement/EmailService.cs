using Azure;
using Azure.Communication.Email;
using CatalogWebApi.DTO;
using CatalogWebApi.Models;



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
                }

                await _emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage); 
               
            }
            catch (RequestFailedException rex)
            {
                // Errores específicos de Azure Communication Services
                Console.WriteLine("Error al enviar email (Azure Communication):");
                Console.WriteLine($"  Status: {rex.Status}");
                Console.WriteLine($"  ErrorCode: {rex.ErrorCode}");
                Console.WriteLine($"  Message: {rex.Message}");
                Console.WriteLine($"  StackTrace: {rex.StackTrace}");

                // Opcional: puedes lanzar de nuevo si quieres que suba a un middleware
                throw;
            }
            catch (Exception ex)
            {
                // Errores generales
                Console.WriteLine("Error general al enviar email:");
                Console.WriteLine($"  Message: {ex.Message}");
                Console.WriteLine($"  InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"  StackTrace: {ex.StackTrace}");

                throw;
            }


        }
    }
}
