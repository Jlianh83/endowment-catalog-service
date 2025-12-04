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
        private readonly IImagesRepository _imagesRepository;

        public ImagesService(BlobServiceClient blobStorageService, IImagesRepository imagesRepository)
        {
            _blobServiceClient = blobStorageService;
            _imagesRepository = imagesRepository;
           
        }

        public async Task<List<int>> createImages(List<IFormFile> images, string[] formats)
        {
            var imagesList = new List<int>();

            if (images.Count() <= 0)
            {
                throw new ArgumentNullException(nameof(images));
            }

            var container = _blobServiceClient.GetBlobContainerClient("uploads");
            await container.CreateIfNotExistsAsync();

            foreach (var i in images)
            {
                var ext = Path.GetExtension(i.FileName);
                if (!formats.Contains(ext))
                {
                    throw new ArgumentException("One of your images has a invalid extention");
                }

                var fileName = $"{Guid.NewGuid().ToString()}{ext}";
                var blob = container.GetBlobClient(fileName);
                using var stream = File.OpenWrite(fileName);
                await blob.UploadAsync(stream);

                var image = new Image
                {
                    Name = fileName,
                    Url = blob.Uri.ToString(),
                };

                await _imagesRepository.SaveFilesAsync(image);

                imagesList.Add(image.Id);    
            }

            return imagesList;

        }
    }
}
