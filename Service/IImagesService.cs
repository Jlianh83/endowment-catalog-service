using CatalogWebApi.Models;

namespace CatalogWebApi.Service
{
    public interface IImagesService
    {
        Task<List<Image>> createImages(List<IFormFile> images, string[] formats);
    }
}
