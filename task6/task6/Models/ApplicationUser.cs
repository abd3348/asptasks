using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace task6.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
    }
}
