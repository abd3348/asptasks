namespace Task4.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
