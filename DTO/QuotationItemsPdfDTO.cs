namespace CatalogWebApi.DTO
{
    public class QuotationItemPdfDTO
    {
        public string EndowmentName { get; set; }
        public string SizeName { get; set; }
        public string ColorName { get; set; }
        public byte[] Images {  get; set; }
        public int Quantity { get; set; }
    }
}
