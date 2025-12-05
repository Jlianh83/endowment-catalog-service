using CatalogWebApi.Data;
using CatalogWebApi.DTO;
using CatalogWebApi.Mapper;
using CatalogWebApi.Models;
using CatalogWebApi.Repository;
using CatalogWebApi.Utils.MapperImplement;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CatalogWebApi.Service.ServiceImplement
{
    public class EndowmentService:IEndowmentService
    {
        private readonly IEndowmentRepository _endowmentRepository;
        private readonly IEndowmentMapper _endowmentMapper;
        private readonly IImagesService _imagesService;
        private readonly Data.AppDbContext _appDbContext;

        public EndowmentService(IEndowmentRepository endowmentRepository, IEndowmentMapper endowmentMapper, Data.AppDbContext appDbContext, IImagesService imagesService) 
        {
            _endowmentRepository = endowmentRepository;
            _endowmentMapper = endowmentMapper;
            _imagesService = imagesService;
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<EndowmentDTO>> GetAllEndowmentsAsync()
        {
            IEnumerable<Endowment> endowments = await _endowmentRepository.GetAllAsync();

            return _endowmentMapper.EntityToDtoList(endowments);

        }

        public async Task<EndowmentDTO> GetEndowmentByIdAsync(int id)
        {
            var endowment = await _endowmentRepository.GetByIdAsync(id);

            return _endowmentMapper.EntitytoDto(endowment); 

        }

        public async Task<IEnumerable<EndowmentDTO>> GetEndowmentByEndowmentTypeIdAsync(int id)
        {
            var endowments = await _endowmentRepository.GetByEndowmentTypeIdAsync(id);

            return _endowmentMapper.EntityToDtoList(endowments);

        }

        public async Task<IEnumerable<EndowmentDTO>> GetEndowmentsByFiltersAsync(FiltersDTO filtersDTO)
        {
            var endowments = await _endowmentRepository.GetFilteredAsync(filtersDTO);

            return _endowmentMapper.EntityToDtoList(endowments);

        }

        public async Task<IEnumerable<EndowmentDTO>> GetEndowmentByNameAsync(NameSeachDTO nameSeachDTO)
        {
            var endowments = await _endowmentRepository.GetEndowmentsByNameAsync(nameSeachDTO);

            return _endowmentMapper.EntityToDtoList(endowments);

        }

        public async Task<EndowmentDTO> CreateEndowmentAsync (SaveEndowmentDTO saveEndowmentDTO)
        {

            var uploadedImages = new List<Image>();

            if (saveEndowmentDTO.UploadedImages != null && saveEndowmentDTO.UploadedImages.Any()) 
            {
                var allowedFormats = new[] { ".jpg", ".jpeg", ".png" };
                uploadedImages = await _imagesService.createImages(saveEndowmentDTO.UploadedImages, allowedFormats);

            }

            var endowment = _endowmentMapper.SaveDtoToEntity(saveEndowmentDTO);

            endowment.Images = uploadedImages;

            await _endowmentRepository.AddAsync(endowment);

            return _endowmentMapper.EntitytoDto(endowment);

        }
        public async Task<EndowmentDTO> UpdateEndowmentAsync(int id, SaveEndowmentDTO saveEndowmentDTO)
        {
            var endowment = await _endowmentRepository.GetByIdAsync(id);

            var uploadedImages = new List<Image>();

            if (endowment == null)
                throw new KeyNotFoundException("Endowment not found");

            if (saveEndowmentDTO.UploadedImages != null && saveEndowmentDTO.UploadedImages.Any())
            {
                var allowedFormats = new[] { ".jpg", ".jpeg", ".png" };
                uploadedImages = await _imagesService.createImages(saveEndowmentDTO.UploadedImages, allowedFormats);

            }

            var modifiedEndowment = _endowmentMapper.SaveDtoToEntity(saveEndowmentDTO);

            modifiedEndowment.Images = uploadedImages;

            await _endowmentRepository.AddAsync(modifiedEndowment);

            return _endowmentMapper.EntitytoDto(modifiedEndowment);

        }

        public async Task DeleteEndowmentAsync(int id)
        {
            await _endowmentRepository.DeleteAsync(id);
        }

    }
}
