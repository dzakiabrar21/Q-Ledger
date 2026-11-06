using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLedger.Models.Entities
{
    public class MachineChecklist
    {
        [Key]
        public int ChecklistId { get; set; }

        public int LineId { get; set; }
        [ForeignKey("LineId")]
        public Line Line { get; set; } = null!;

        public DateOnly CheckDate { get; set; }

        public int Shift { get; set; } // 1, 2, 3

        public bool IsPassed { get; set; } = true;

        public int NgCount { get; set; } = 0;

        [MaxLength(255)]
        public string? Remarks { get; set; }

        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    }
}

