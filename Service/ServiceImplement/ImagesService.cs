using Azure.Storage.Blobs;
using CatalogWebApi.Data;
using CatalogWebApi.Models;
using CatalogWebApi.Repository;
using Microsoft.AspNetCore.Hosting;

namespace CatalogWebApi.Service.ServiceImplement
{
    public class ImagesService : IImagesService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public ImagesService(BlobServiceClient blobStorageService)
        {
            _blobServiceClient = blobStorageService;
           
        }

        public async Task<List<Image>> createImages(List<IFormFile> images, string[] formats)
        {
            var imagesList = new List<Image>();

            foreach (var i in images)
            {
                var ext = Path.GetExtension(i.FileName).ToLower();
                if (!formats.Contains(ext))
                {
                    throw new ArgumentException("One of your images has a invalid extention");
                }

                var fileName = $"{Guid.NewGuid().ToString()}{ext}";

                var container = _blobServiceClient.GetBlobContainerClient("uploads");
                await container.CreateIfNotExistsAsync();

                var blob = container.GetBlobClient(fileName);
                using var stream = i.OpenReadStream();
                await blob.UploadAsync(stream, overwrite: true);

                imagesList.Add(new Image
                {
                    Name = i.Name,
                    Url = blob.Uri.ToString(),

                });
            }

            return imagesList;

        }
    }
}
