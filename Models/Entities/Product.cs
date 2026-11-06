using System.ComponentModel.DataAnnotations;

namespace QLedger.Models.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ItemCode { get; set; } = string.Empty; // e.g. "G2R-1-24VDC"

        [Required]
        [MaxLength(150)]
        public string ItemName { get; set; } = string.Empty; // e.g. "Power Relay G2R Series"

        [MaxLength(50)]
        public string Category { get; set; } = "Relay";

        public int DefaultSamplingQty { get; set; } = 50;

        public bool IsActive { get; set; } = true;

        public ICollection<InspectionLedger> InspectionLedgers { get; set; } = new List<InspectionLedger>();
    }
}

