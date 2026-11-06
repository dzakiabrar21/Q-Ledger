using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLedger.Models.Entities
{
    public class InspectionLedger
    {
        [Key]
        public long LedgerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderNo { get; set; } = string.Empty; // Manufacturing Order (MO)

        public int LineId { get; set; }
        [ForeignKey("LineId")]
        public Line Line { get; set; } = null!;

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LotNo { get; set; } = string.Empty;

        public int OrderQty { get; set; }

        public int QueueNumber { get; set; } // Per-area sequential number

        // Status: 'WAITING', 'UNDER_INSPECTION', 'FINISH_OK', 'HOLD'
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "WAITING";

        // Quality Document Flags
        public bool IsARI { get; set; } = false;   // Abnormal Release Instruction
        public bool IsNRS { get; set; } = false;   // Non-conformance Report
        public bool IsIFC { get; set; } = false;   // Initial Flow Control
        public bool IsRRS { get; set; } = false;   // Re-inspection Record Sheet
        public bool IsEFR { get; set; } = false;   // Engineering Failure Report
        public bool IsSplit { get; set; } = false; // Split Lot
        public bool IsTopup { get; set; } = false; // Topup Qty

        // Production Handover Data
        public int PdPicId { get; set; }
        [ForeignKey("PdPicId")]
        public User PdPic { get; set; } = null!;

        public DateTime PdInTime { get; set; } = DateTime.UtcNow;

        // QA Inspection Data
        public int? QaInspectorId { get; set; }
        [ForeignKey("QaInspectorId")]
        public User? QaInspector { get; set; }

        [MaxLength(20)]
        public string? InspectionLevel { get; set; } = "NORMAL"; // 'NORMAL', 'TIGHTENED', 'REDUCED'

        public DateTime? QaStartTime { get; set; }
        public DateTime? QaFinishTime { get; set; }

        [MaxLength(10)]
        public string? QaJudgement { get; set; } // 'OK', 'NG'

        [MaxLength(500)]
        public string? Remark { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation to NrsReport (if Judgement == 'NG')
        public NrsReport? NrsReport { get; set; }

        // Helper properties for calculation
        [NotMapped]
        public double? WaitingDurationMinutes => QaStartTime.HasValue 
            ? (QaStartTime.Value - PdInTime).TotalMinutes 
            : (DateTime.UtcNow - PdInTime).TotalMinutes;

        [NotMapped]
        public double? InspectionDurationMinutes => (QaStartTime.HasValue && QaFinishTime.HasValue) 
            ? (QaFinishTime.Value - QaStartTime.Value).TotalMinutes 
            : null;
    }
}

