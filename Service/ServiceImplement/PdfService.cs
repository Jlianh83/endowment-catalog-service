using Azure.Storage.Blobs;
using CatalogWebApi.Data;
using CatalogWebApi.DTO;
using CatalogWebApi.Infrastructure.Pdf;
using QuestPDF.Fluent;

namespace CatalogWebApi.Service.ServiceImplement
{
    public class PdfService : IPdfService
    {

        private readonly AppDbContext _appDbContext;
        private readonly BlobServiceClient _blobServiceClient;
        public PdfService(AppDbContext appDbContext, BlobServiceClient blobServiceClient) 
        {
            _appDbContext = appDbContext;
            _blobServiceClient = blobServiceClient;
        }
        public async Task<byte[]> GenerateQuotationPdf(QuotationDTO quotationDTO)
        {

            var items = new List<QuotationItemPdfDTO>();

            foreach(var item in quotationDTO.quotationItems)
            {
                var endowment = await _appDbContext.Endowments.FindAsync(item.endowmentId);
                var size = await _appDbContext.Sizes.FindAsync(item.sizeId);
                var color = await _appDbContext.Colors.FindAsync(item.colorId);

                byte[] imgBytes = Array.Empty<byte>();
                if (!string.IsNullOrEmpty(item.imageName))
                {
                    imgBytes = await GetBlobBytesAsync(item.imageName);
                }

                Console.WriteLine($"Item: {item.endowmentId} {item.sizeId} {item.colorId} {item.quantity}");

                items.Add(new QuotationItemPdfDTO
                {
                    EndowmentName = endowment.Name,
                    SizeName = size.Name,
                    ColorName = color.Name,
                    Quantity = item.quantity,
                    Images = imgBytes
                });
            }

            var pdfModel = new QuotationPdfDTO
            {
                clientName = quotationDTO.clientName,
                clientCompany = quotationDTO.clientCompany,
                clientEmail = quotationDTO.clientEmail,
                clientPhone = quotationDTO.clientPhone,
                createdAt = quotationDTO.createdAt,
                Items = items
            };

            var buildPdf = new QuotationPdf(pdfModel);
            var bytes = buildPdf.GeneratePdf();

            return bytes;

        }

        private async Task<byte[]> GetBlobBytesAsync(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                return Array.Empty<byte>();

            var container = _blobServiceClient.GetBlobContainerClient("uploads");
            var blob = container.GetBlobClient(blobName);

            if (!await blob.ExistsAsync())
                return Array.Empty<byte>();

            using var ms = new MemoryStream();
            await blob.DownloadToAsync(ms);
            return ms.ToArray();
        }

    }
}
