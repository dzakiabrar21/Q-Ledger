using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLedger.Data;
using QLedger.Models.ViewModels;

namespace QLedger.Controllers
{
    public class QueueController : Controller
    {
        private readonly QLedgerDbContext _context;

        public QueueController(QLedgerDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await GetQueueBoardDataAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetQueueData()
        {
            var model = await GetQueueBoardDataAsync();
            return Json(new
            {
                areaSummaries = model.AreaSummaries.Select(a => new
                {
                    areaName = a.AreaName,
                    inProgressCount = a.InProgressCount,
                    bgHeaderColor = a.BgHeaderColor,
                    bgBodyColor = a.BgBodyColor,
                    nextLot = a.NextLot == null ? null : new
                    {
                        queueNumber = a.NextLot.QueueNumber.ToString("D2"),
                        orderNo = a.NextLot.OrderNo,
                        typeCode = a.NextLot.Product.ItemCode,
                        typeName = a.NextLot.Product.ItemName,
                        lotNo = a.NextLot.LotNo,
                        orderQty = a.NextLot.OrderQty
                    }
                }),
                totalWaiting = model.TotalWaiting,
                totalInProgress = model.TotalInProgress,
                totalFinishedToday = model.TotalFinishedToday
            });
        }

        private async Task<QueueBoardViewModel> GetQueueBoardDataAsync()
        {
            var today = DateTime.UtcNow.Date;

            var waitingLots = await _context.InspectionLedgers
                .Include(x => x.Line)
                .Include(x => x.Product)
                .Include(x => x.PdPic)
                .Where(x => x.Status == "WAITING")
                .OrderBy(x => x.PdInTime)
                .ToListAsync();

            var inProgressLots = await _context.InspectionLedgers
                .Include(x => x.Line)
                .Include(x => x.Product)
                .Include(x => x.QaInspector)
                .Where(x => x.Status == "UNDER_INSPECTION")
                .OrderBy(x => x.QaStartTime)
                .ToListAsync();

            var recentFinished = await _context.InspectionLedgers
                .Include(x => x.Line)
                .Include(x => x.Product)
                .Include(x => x.QaInspector)
                .Where(x => (x.Status == "FINISH_OK" || x.Status == "HOLD") && x.PdInTime >= today.AddDays(-1))
                .OrderByDescending(x => x.QaFinishTime)
                .Take(10)
                .ToListAsync();

            // Distinct Areas: SWITCH 1, RELAY 1, RELAY 2
            var areas = new[]
            {
                new { Name = "SWITCH 1", Header = "#CC9900", Body = "#FF9900" },
                new { Name = "RELAY 1", Header = "#17AD45", Body = "#1ABC4C" },
                new { Name = "RELAY 2", Header = "#308ded", Body = "#3699ff" }
            };

            var areaSummaries = new List<AreaQueueSummary>();
            foreach (var a in areas)
            {
                var nextLot = waitingLots.FirstOrDefault(x => x.Line.Area == a.Name);
                var inProgressCount = inProgressLots.Count(x => x.Line.Area == a.Name);

                areaSummaries.Add(new AreaQueueSummary
                {
                    AreaName = a.Name,
                    InProgressCount = inProgressCount,
                    NextLot = nextLot,
                    BgHeaderColor = a.Header,
                    BgBodyColor = a.Body
                });
            }

            return new QueueBoardViewModel
            {
                AreaSummaries = areaSummaries,
                WaitingList = waitingLots,
                InProgressList = inProgressLots,
                RecentFinishedList = recentFinished
            };
        }
    }
}

