using CatalogWebApi.Models;
using System;

namespace CatalogWebApi.DTO
{
    public class EndowmentDTO
    {
        public int id {  get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int type { get; set; }
        public int category { get; set; }
        public List<ImagesDTO> images { get; set; } = new();

    }
}
