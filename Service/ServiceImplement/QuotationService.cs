using CatalogWebApi.DTO;
using CatalogWebApi.Mapper;
using CatalogWebApi.Models;
using CatalogWebApi.Repository;

namespace CatalogWebApi.Service.ServiceImplement
{
    public class QuotationService : IQuotationsService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly IQuotationMapper _quotationMapper;
        private readonly IEmailService _emailService;
        private readonly IPdfService _pdfService;
        public QuotationService(IQuotationRepository quotationRepository, IQuotationMapper quotationMapper, IEmailService emailService, IPdfService pdfService) 
        { 
            _quotationRepository = quotationRepository;
            _quotationMapper = quotationMapper;
            _emailService = emailService;
            _pdfService = pdfService;
        }

        public async Task<IEnumerable<QuotationDTO>> GetAllQuotationsAsync()
        {
            IEnumerable<Quotation> endowments = await _quotationRepository.GetAllAsync();

            return _quotationMapper.EntityToDtoList(endowments);
        }
        public async Task<QuotationDTO> GetQuotationByIdAsync(int id)
        {
            var quotation = await _quotationRepository.GetByIdAsync(id);

            return _quotationMapper.EntitytoDto(quotation);

        }
        public async Task<QuotationDTO> CreateQuotationAsync(QuotationDTO quotationDTO)
        {

            var quotation = _quotationMapper.DtoToEntity(quotationDTO);

            quotation = await _quotationRepository.AddAsync(quotation);

            var itemsList = quotationDTO.quotationItems.Select(q => _quotationMapper.DtoToItemEntity(q, quotation.Id)).ToList();

            await _quotationRepository.AddQuotationItemsAsync(itemsList);

            quotation.QuotationItems = itemsList;

            byte[] quotationPdf = await _pdfService.GenerateQuotationPdf(quotationDTO);

            var emailInfo = new SendQuotationDTO
            {
                To = quotationDTO.clientEmail,
                Subject = "Cotizacion de dotaciones"
            };

            var fileName = $"Cotizacion_{quotation.ClientName}_{quotation.CreatedAt}.pdf";

            await _emailService.sendEmail(emailInfo, quotationPdf, fileName, quotation.ClientName);

            return _quotationMapper.EntitytoDto(quotation);

        }

    }
}
