namespace Task4.Models
{
    public class SelectedOfferViewModel
    {
        public string CategoryName { get; set; } = "";
        public string SupplierName { get; set; } = "";
        public string OfferName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotal { get; set; }
    }
}
