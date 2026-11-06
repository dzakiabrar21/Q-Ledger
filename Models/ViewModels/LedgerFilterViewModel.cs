using QLedger.Models.Entities;

namespace QLedger.Models.ViewModels
{
    public class LedgerFilterViewModel
    {
        public DateTime? StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime? EndDate { get; set; } = DateTime.Today;
        public int? LineId { get; set; }
        public string? Status { get; set; } // ALL, WAITING, UNDER_INSPECTION, FINISH_OK, HOLD
        public string? Judgement { get; set; } // ALL, OK, NG
        public string? TagFilter { get; set; } // ALL, ARI, IFC, RRS, EFR, SPLIT, TOPUP

        public List<InspectionLedger> Items { get; set; } = new();

        // Statistics
        public int TotalLots => Items.Count;
        public int TotalOk => Items.Count(x => x.QaJudgement == "OK");
        public int TotalNg => Items.Count(x => x.QaJudgement == "NG");
        public double PassRate => TotalLots > 0 ? ((double)TotalOk / TotalLots) * 100 : 0;
    }
}

