using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLedger.Models.Entities
{
    public class NrsReport
    {
        [Key]
        public long NrsId { get; set; }

        [Required]
        [MaxLength(30)]
        public string NrsNumber { get; set; } = string.Empty; // e.g. "NRS-202611-001"

        public long LedgerId { get; set; }
        [ForeignKey("LedgerId")]
        public InspectionLedger Ledger { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string DefectCategory { get; set; } = string.Empty; // e.g. "Dimensi", "Fungsi Kontak", "Kaki Bengkok"

        public int DefectQty { get; set; } = 1;

        // Status: 'OPEN', 'PENDING_APPROVAL', 'CLOSED'
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "OPEN";

        // 5-Why Analysis: Occurrence (Mengapa terjadi di produksi?)
        [MaxLength(255)]
        public string? WhyOccur1 { get; set; }
        [MaxLength(255)]
        public string? WhyOccur2 { get; set; }
        [MaxLength(255)]
        public string? WhyOccur3 { get; set; }
        [MaxLength(255)]
        public string? WhyOccur4 { get; set; }
        [MaxLength(255)]
        public string? WhyOccur5 { get; set; } // Root cause occurrence

        // 5-Why Analysis: Outflow (Mengapa lolos sampai ke QA?)
        [MaxLength(255)]
        public string? WhyOutflow1 { get; set; }
        [MaxLength(255)]
        public string? WhyOutflow2 { get; set; }
        [MaxLength(255)]
        public string? WhyOutflow3 { get; set; } // Root cause detection

        // Countermeasures (Rencana Tindakan)
        [MaxLength(500)]
        public string? TemporaryAction { get; set; } // Tindakan Cepat (Containment)

        [MaxLength(500)]
        public string? PermanentAction { get; set; } // Tindakan Pencegahan Permanen

        public DateOnly? TargetDate { get; set; }

        public int? PicUserId { get; set; }
        [ForeignKey("PicUserId")]
        public User? PicUser { get; set; }

        // Disposition (Nasib Fisik Barang: 'REWORK', 'SCRAP', 'SPECIAL_RELEASE')
        [MaxLength(30)]
        public string? Disposition { get; set; }

        // Supervisor Approval
        public int? ApprovedById { get; set; }
        [ForeignKey("ApprovedById")]
        public User? ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        [MaxLength(500)]
        public string? SupervisorNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

