using Microsoft.AspNetCore.Identity;

namespace task6.Models
{
    public class SupplyRequest
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";

        public int Id { get; set; }

        public DateTime RequestDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = Pending;

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public ICollection<RequestItem> RequestItems { get; set; }
            = new List<RequestItem>();
    }
}
