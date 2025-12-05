using CatalogWebApi.DTO;
using CatalogWebApi.Models;

namespace CatalogWebApi.Mapper.MapperImplement
{
    public class QuotationMapper : IQuotationMapper
    {
        public Quotation DtoToEntity(QuotationDTO quotationDTO)
        {
            return new Quotation
            {
                Id = quotationDTO.id,
                ClientName = quotationDTO.clientName,
                ClientAddress = quotationDTO.clientAddress,
                ClientCompany = quotationDTO.clientCompany,
                ClientEmail = quotationDTO.clientEmail,
                ClientPhone = quotationDTO.clientPhone,
                CreatedAt = quotationDTO.createdAt,
            }; 
        }

        public QuotationItem DtoToItemEntity(QuotationItemsDTO quotationItemsDTO, int quotationId)
        {
            return new QuotationItem
            {
                Id = quotationItemsDTO.id,
                QuotationId = quotationId,
                EndowmentId = quotationItemsDTO.endowmentId,
                ColorId = quotationItemsDTO.colorId,
                SizeId = quotationItemsDTO.sizeId,
                Quantity = quotationItemsDTO.quantity,
                ImageItemName = quotationItemsDTO.imageName
                
            };
        }

        public QuotationDTO EntitytoDto(Quotation quotation)
        {
            return new QuotationDTO
            {
                id = quotation.Id,
                clientName = quotation.ClientName,
                clientAddress = quotation.ClientAddress,
                clientCompany = quotation.ClientCompany,
                clientEmail = quotation.ClientEmail,
                clientPhone = quotation.ClientPhone,
                createdAt = quotation.CreatedAt,
                quotationItems = quotation.QuotationItems.Select(q => new QuotationItemsDTO
                {
                    id = q.Id,
                    endowmentId = q.Id,
                    quotationId = q.QuotationId,
                    colorId = q.ColorId,
                    sizeId = q.SizeId,
                    quantity = q.Quantity
                }).ToList()
            };
        }

        public IEnumerable<QuotationDTO> EntityToDtoList(IEnumerable<Quotation> quotations)
        {
            return quotations.Select(quotations => EntitytoDto(quotations));
        }
    }
}
