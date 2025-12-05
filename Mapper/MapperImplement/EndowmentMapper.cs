using CatalogWebApi.DTO;
using CatalogWebApi.Mapper;
using CatalogWebApi.Models;
using System;

namespace CatalogWebApi.Utils.MapperImplement
{
    public class EndowmentMapper : IEndowmentMapper
    {
        public Endowment SaveDtoToEntity(SaveEndowmentDTO dto)
        {
            return new Endowment
            {
                Id = dto.id,
                Name = dto.name,
                Description = dto.description,
                EndowmentTypeId = dto.typeId,
                EndowmentCategoryId = dto.categoryId,
                Images = dto.images.Select(id => new Image { Id = id }).ToList()
                
            };
        }

        public Endowment DtoToEntity(EndowmentDTO endowmentDTO)
        {
            return new Endowment
            {
                Id = endowmentDTO.id,
                Name = endowmentDTO.name,
                Description = endowmentDTO.description,
                EndowmentTypeId = endowmentDTO.type,
                EndowmentCategoryId = endowmentDTO.category,
                Images = endowmentDTO.images.Select(id => new Image { Id = id.id }).ToList()
            };
        }

        public EndowmentDTO EntitytoDto(Endowment endowment)
        {
            return new EndowmentDTO
            {
                id = endowment.Id,
                name = endowment.Name,
                description = endowment.Description,
                type = endowment.EndowmentTypeId,
                category = endowment.EndowmentCategoryId,
                images = endowment.Images.Select(i => new ImagesDTO
                {
                    id = i.Id,
                    name = i.Name,
                    url = i.Url
                }).ToList()
            };
        }
        public IEnumerable<EndowmentDTO> EntityToDtoList(IEnumerable<Endowment> endowments)
        {
            return endowments.Select(endowments => EntitytoDto(endowments));
        }
    }
}
