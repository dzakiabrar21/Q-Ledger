using System.ComponentModel.DataAnnotations;

namespace QLedger.Models.ViewModels
{
    public class HandoverCreateViewModel
    {
        [Required(ErrorMessage = "Nomor MO wajib diisi")]
        [Display(Name = "Nomor MO (Manufacturing Order)")]
        public string OrderNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pilih Lini Produksi")]
        [Display(Name = "Lini Produksi")]
        public int LineId { get; set; }

        [Required(ErrorMessage = "Pilih Tipe Produk")]
        [Display(Name = "Tipe Produk")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Nomor Lot wajib diisi")]
        [Display(Name = "Nomor Lot")]
        public string LotNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kuantitas Order wajib diisi")]
        [Range(1, 1000000, ErrorMessage = "Kuantitas minimal 1 pcs")]
        [Display(Name = "Kuantitas (pcs)")]
        public int OrderQty { get; set; }

        [Required(ErrorMessage = "Pilih PIC Penyerah")]
        [Display(Name = "PIC Produksi")]
        public int PdPicId { get; set; }

        // Quality Tagging
        public bool IsARI { get; set; }
        public bool IsNRS { get; set; }
        public bool IsIFC { get; set; }
        public bool IsRRS { get; set; }
        public bool IsEFR { get; set; }
        public bool IsSplit { get; set; }
        public bool IsTopup { get; set; }

        [Display(Name = "Catatan Tambahan")]
        public string? Remark { get; set; }
    }
}

