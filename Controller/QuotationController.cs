using CatalogWebApi.DTO;
using CatalogWebApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace CatalogWebApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotationController : ControllerBase
    {
        private readonly IQuotationsService _quotationsService;

        public QuotationController(IQuotationsService quotationsService)
        {
            _quotationsService = quotationsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuotationDTO>>> GetAll()
        {
            var results = await _quotationsService.GetAllQuotationsAsync();
            return Ok(results);
        }
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<QuotationDTO>> GetById(int id)
        {
            var endowment = await _quotationsService.GetQuotationByIdAsync(id);
            if (endowment == null) return NotFound();
            return Ok(endowment);
        }
        [HttpPost]
        public async Task<ActionResult<QuotationDTO>> Create([FromBody] QuotationDTO quotationDTO)
        {
            var quotation = await _quotationsService.CreateQuotationAsync(quotationDTO);
            return CreatedAtAction(nameof(GetById), new { id = quotation.id }, quotation);
        }
    }
}
