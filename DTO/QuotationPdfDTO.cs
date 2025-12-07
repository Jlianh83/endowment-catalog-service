namespace CatalogWebApi.DTO
{
    public class QuotationPdfDTO
    {
        public string clientName { get; set; }
        public string clientCompany { get; set; }
        public string clientEmail { get; set; }
        public string clientPhone { get; set; }
        public DateOnly createdAt { get; set; }
        public byte[] companyInfo { get; set; }

        public List<QuotationItemPdfDTO> Items { get; set; }
    }

}
