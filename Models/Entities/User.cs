using System.ComponentModel.DataAnnotations;

namespace QLedger.Models.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "PRODUCTION"; // "PRODUCTION", "INSPECTOR", "SUPERVISOR", "ADMIN"

        [MaxLength(50)]
        public string? RfidTag { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

