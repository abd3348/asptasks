using System.ComponentModel.DataAnnotations.Schema;

namespace CafeteriaCodeFirst.Models
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

        public Product Product { get; set; }
    }
}