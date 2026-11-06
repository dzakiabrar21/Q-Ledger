using QLedger.Models.Entities;

namespace QLedger.Models.ViewModels
{
    public class AreaQueueSummary
    {
        public string AreaName { get; set; } = string.Empty;
        public int InProgressCount { get; set; }
        public InspectionLedger? NextLot { get; set; }
        public string BgHeaderColor { get; set; } = "#CC9900";
        public string BgBodyColor { get; set; } = "#FF9900";
    }

    public class QueueBoardViewModel
    {
        public List<AreaQueueSummary> AreaSummaries { get; set; } = new();
        public List<InspectionLedger> WaitingList { get; set; } = new();
        public List<InspectionLedger> InProgressList { get; set; } = new();
        public List<InspectionLedger> RecentFinishedList { get; set; } = new();

        public int TotalWaiting => WaitingList.Count;
        public int TotalInProgress => InProgressList.Count;
        public int TotalFinishedToday => RecentFinishedList.Count;
    }
}

