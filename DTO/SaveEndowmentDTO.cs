using Swashbuckle;

namespace CatalogWebApi.DTO
{
    public class SaveEndowmentDTO
    {
        public int id {  get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public int typeId { get; set; } = new();
        public int categoryId { get; set; } = new();
        public List<IFormFile> UploadedImages { get; set; } = new List<IFormFile>();
        public List<int> images { get; set; } = new();

    }
}
