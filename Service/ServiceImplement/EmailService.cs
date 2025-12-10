using CatalogWebApi.DTO;
using CatalogWebApi.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CatalogWebApi.Service.ServiceImplement
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IEmailTemplateBuilder _emailTemplateBuilder;
        private readonly string _logicAppUrl =
            "https://prod-16.canadacentral.logic.azure.com:443/workflows/78cff9fc205c4c79b4d229e9b692b7f4/triggers/When_an_HTTP_request_is_received/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2FWhen_an_HTTP_request_is_received%2Frun&sv=1.0&sig=44yBilr21cGFf_QMcWhr4M2P-edSVhG-cTu1SqnRg8I";

        public EmailService(IEmailTemplateBuilder emailTemplateBuilder, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _emailTemplateBuilder = emailTemplateBuilder;
        }

        public async Task sendEmail(SendQuotationDTO sendQuotationDTO, byte[] file, string fileName, string clientName)
        {
            try
            {
                // 1. HTML generado
                var html = await _emailTemplateBuilder.BuildCotizationTemplateAsync(clientName);

                // 2. Convertir PDF a Base64
                //var base64File = Convert.ToBase64String(file);

                // 3. Construir el body que espera la Logic App
                var payload = new
                {
                    to = sendQuotationDTO.To,
                    subject = sendQuotationDTO.Subject,
                    html = html,
                };

                var json = JsonSerializer.Serialize(payload);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 4. Enviar request a Logic App
                var response = await _httpClient.PostAsync(_logicAppUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error enviando LogicApp: " + error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando correo vía Logic App: {ex.Message}");
            }
        }
    }
}
