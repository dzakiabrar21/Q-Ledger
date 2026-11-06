using System.ComponentModel.DataAnnotations;

namespace QLedger.Models.Entities
{
    public class Line
    {
        [Key]
        public int LineId { get; set; }

        [Required]
        [MaxLength(20)]
        public string LineCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LineName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Area { get; set; } = string.Empty; // e.g. "SWITCH 1", "RELAY 1", "RELAY 2"

        public bool IsActive { get; set; } = true;

        public ICollection<InspectionLedger> InspectionLedgers { get; set; } = new List<InspectionLedger>();
        public ICollection<MachineChecklist> MachineChecklists { get; set; } = new List<MachineChecklist>();
    }
}

