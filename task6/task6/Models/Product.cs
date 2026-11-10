using System.ComponentModel.DataAnnotations;

namespace task6.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public Category? Category { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
