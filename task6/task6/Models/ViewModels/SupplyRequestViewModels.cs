using System.ComponentModel.DataAnnotations;

namespace task6.Models.ViewModels
{
    public class RequestProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class SupplyRequestFormViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; } = SupplyRequest.Pending;
        public List<RequestProductViewModel> Products { get; set; } = new();
    }
}
