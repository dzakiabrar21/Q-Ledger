using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QLedger.Data;
using QLedger.Hubs;
using QLedger.Models.Entities;
using QLedger.Models.ViewModels;

namespace QLedger.Controllers
{
    public class InspectionController : Controller
    {
        private readonly QLedgerDbContext _context;
        private readonly IHubContext<QueueHub> _hubContext;

        public InspectionController(QLedgerDbContext context, IHubContext<QueueHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var activeLots = await _context.InspectionLedgers
                .Include(x => x.Line)
                .Include(x => x.Product)
                .Include(x => x.PdPic)
                .Include(x => x.QaInspector)
                .Where(x => x.Status == "WAITING" || x.Status == "UNDER_INSPECTION")
                .OrderBy(x => x.Status == "UNDER_INSPECTION" ? 0 : 1)
                .ThenBy(x => x.PdInTime)
                .ToListAsync();

            ViewBag.Inspectors = new SelectList(
                await _context.Users.Where(x => x.Role == "INSPECTOR" || x.Role == "SUPERVISOR" || x.Role == "ADMIN").ToListAsync(),
                "UserId",
                "FullName"
            );

            return View(activeLots);
        }

        [HttpPost]
        public async Task<IActionResult> StartInspection([FromBody] StartInspectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Data tidak valid." });
            }

            var ledger = await _context.InspectionLedgers.FindAsync(model.LedgerId);
            if (ledger == null)
            {
                return Json(new { success = false, message = "Lot tidak ditemukan." });
            }

            if (ledger.Status != "WAITING")
            {
                return Json(new { success = false, message = $"Status lot saat ini adalah '{ledger.Status}', tidak dapat dimulai." });
            }

            ledger.Status = "UNDER_INSPECTION";
            ledger.QaStartTime = DateTime.UtcNow;
            ledger.QaInspectorId = model.InspectorId;
            ledger.InspectionLevel = model.InspectionLevel;

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("QueueUpdated");

            return Json(new { success = true, message = "Inspeksi dimulai. Stopwatch waktu berjalan!" });
        }

        [HttpPost]
        public async Task<IActionResult> FinishInspection([FromBody] FinishInspectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Data putusan tidak valid." });
            }

            var ledger = await _context.InspectionLedgers
                .Include(x => x.Line)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.LedgerId == model.LedgerId);

            if (ledger == null)
            {
                return Json(new { success = false, message = "Lot tidak ditemukan." });
            }

            if (ledger.Status != "UNDER_INSPECTION")
            {
                return Json(new { success = false, message = "Lot harus dalam status 'UNDER_INSPECTION' untuk diselesaikan." });
            }

            ledger.QaFinishTime = DateTime.UtcNow;
            ledger.QaJudgement = model.Judgement.ToUpper();
            ledger.Remark = model.Remark;

            if (ledger.QaJudgement == "OK")
            {
                ledger.Status = "FINISH_OK";
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("QueueUpdated");

                return Json(new
                {
                    success = true,
                    status = "OK",
                    message = $"Lot MO {ledger.OrderNo} dinyatakan PASS (OK)! Barang siap dirilis/dikirim."
                });
            }
            else // Judgement == "NG"
            {
                ledger.Status = "HOLD";

                // Generate new unique NRS ticket
                var currentMonth = DateTime.UtcNow.ToString("yyyyMM");
                var countThisMonth = await _context.NrsReports
                    .CountAsync(x => x.NrsNumber.StartsWith($"NRS-{currentMonth}"));

                var nrsNumber = $"NRS-{currentMonth}-{(countThisMonth + 1):D4}";

                var nrs = new NrsReport
                {
                    NrsNumber = nrsNumber,
                    LedgerId = ledger.LedgerId,
                    DefectCategory = string.IsNullOrWhiteSpace(model.DefectCategory) ? "Visual / Fungsi Cacat" : model.DefectCategory,
                    DefectQty = model.DefectQty > 0 ? model.DefectQty : 1,
                    Status = "OPEN",
                    CreatedAt = DateTime.UtcNow
                };

                _context.NrsReports.Add(nrs);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("QueueUpdated");

                return Json(new
                {
                    success = true,
                    status = "HOLD",
                    nrsId = nrs.NrsId,
                    nrsNumber = nrs.NrsNumber,
                    message = $"Lot MO {ledger.OrderNo} dinyatakan REJECT (NG)! Status otomatis menjadi HOLD dan tiket {nrsNumber} telah dibuat."
                });
            }
        }
    }
}

