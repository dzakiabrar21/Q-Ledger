using System.ComponentModel.DataAnnotations;

namespace QLedger.Models.ViewModels
{
    public class StartInspectionViewModel
    {
        [Required]
        public long LedgerId { get; set; }

        [Required(ErrorMessage = "Pilih Inspector atau Tap RFID")]
        public int InspectorId { get; set; }

        public string? RfidTag { get; set; }

        [Required(ErrorMessage = "Pilih Inspection Level")]
        public string InspectionLevel { get; set; } = "NORMAL"; // 'NORMAL', 'TIGHTENED', 'REDUCED'
    }

    public class FinishInspectionViewModel
    {
        [Required]
        public long LedgerId { get; set; }

        [Required(ErrorMessage = "Pilih Putusan Mutu (OK atau NG)")]
        public string Judgement { get; set; } = "OK"; // "OK", "NG"

        public string? Remark { get; set; }

        // Optional defect fields if NG
        public string? DefectCategory { get; set; }
        public int DefectQty { get; set; } = 1;
    }
}

