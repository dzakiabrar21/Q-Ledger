using QLedger.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace QLedger.Models.ViewModels
{
    public class NrsInvestigationViewModel
    {
        [Required]
        public long NrsId { get; set; }

        public NrsReport? Report { get; set; }

        // 5-Why Occurrence
        [Required(ErrorMessage = "Why 1 (Occurrence) wajib diisi")]
        public string WhyOccur1 { get; set; } = string.Empty;
        public string? WhyOccur2 { get; set; }
        public string? WhyOccur3 { get; set; }
        public string? WhyOccur4 { get; set; }
        public string? WhyOccur5 { get; set; }

        // 5-Why Outflow
        [Required(ErrorMessage = "Why 1 (Outflow) wajib diisi")]
        public string WhyOutflow1 { get; set; } = string.Empty;
        public string? WhyOutflow2 { get; set; }
        public string? WhyOutflow3 { get; set; }

        // Actions
        [Required(ErrorMessage = "Tindakan Sementara (Containment) wajib diisi")]
        public string TemporaryAction { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tindakan Permanen wajib diisi")]
        public string PermanentAction { get; set; } = string.Empty;

        [Required(ErrorMessage = "Target Tanggal wajib ditentukan")]
        public DateOnly TargetDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));

        [Required(ErrorMessage = "Pilih PIC Penanggung Jawab")]
        public int PicUserId { get; set; }

        [Required(ErrorMessage = "Pilih Disposisi Fisik")]
        public string Disposition { get; set; } = "REWORK"; // "REWORK", "SCRAP", "SPECIAL_RELEASE"
    }

    public class NrsApprovalViewModel
    {
        [Required]
        public long NrsId { get; set; }

        [Required(ErrorMessage = "Pilih Supervisor Approver")]
        public int SupervisorId { get; set; }

        public string? SupervisorNotes { get; set; }

        public bool IsApproved { get; set; } = true;
    }
}

