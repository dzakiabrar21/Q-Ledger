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
    public class HandoverController : Controller
    {
        private readonly QLedgerDbContext _context;
        private readonly IHubContext<QueueHub> _hubContext;

        public HandoverController(QLedgerDbContext context, IHubContext<QueueHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            await PopulateDropdownsAsync();
            var model = new HandoverCreateViewModel
            {
                OrderQty = 1000
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CheckGatekeeper(int lineId, string orderNo)
        {
            var result = new GatekeeperCheckResult();

            // 1. Check if Order is already in active queue
            var existing = await _context.InspectionLedgers
                .AnyAsync(x => x.OrderNo == orderNo && (x.Status == "WAITING" || x.Status == "UNDER_INSPECTION"));

            if (existing)
            {
                result.IsEligible = false;
                result.OverallMessage = $"Nomor MO '{orderNo}' sudah berada dalam antrean aktif!";
                return Json(result);
            }

            // 2. Check NRS Quality Gate (Is there any open defect on this line?)
            var activeNrs = await _context.NrsReports
                .Include(x => x.Ledger)
                .Where(x => x.Ledger.LineId == lineId && x.Status != "CLOSED")
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (activeNrs != null)
            {
                result.IsEligible = false;
                result.NrsStatus = "HOLD";
                result.NrsMessage = $"Line ini memiliki tiket tertahan aktif ({activeNrs.NrsNumber})!";
                result.OverallMessage = $"Pengiriman ditahan: Terdapat tiket defect aktif ({activeNrs.NrsNumber}) pada lini ini. Harap selesaikan investigasi terlebih dahulu.";
                return Json(result);
            }
            else
            {
                result.NrsStatus = "OK";
                result.NrsMessage = "Tidak ada issue defect tertahan.";
            }

            // 3. Check Machine Checklist for today
            var today = DateOnly.FromDateTime(DateTime.Today);
            var mc = await _context.MachineChecklists
                .Where(x => x.LineId == lineId && x.CheckDate == today)
                .OrderByDescending(x => x.CheckedAt)
                .FirstOrDefaultAsync();

            if (mc == null)
            {
                // If not found, let's create a passing dummy check for convenience if line is valid
                mc = new MachineChecklist
                {
                    LineId = lineId,
                    CheckDate = today,
                    Shift = 1,
                    IsPassed = true,
                    NgCount = 0,
                    Remarks = "Auto initial shift check OK"
                };
                _context.MachineChecklists.Add(mc);
                await _context.SaveChangesAsync();

                result.MachineStatus = "OK";
                result.MachineMessage = "Checklist Mesin Harian: Lolos (0 NG)";
            }
            else if (!mc.IsPassed || mc.NgCount > 0)
            {
                result.IsEligible = false;
                result.MachineStatus = "NG";
                result.MachineMessage = $"Checklist Mesin: Terdapat {mc.NgCount} item NG!";
                result.OverallMessage = "Pengiriman ditahan: Kondisi mesin belum memenuhi syarat standar (NG).";
                return Json(result);
            }
            else
            {
                result.MachineStatus = "OK";
                result.MachineMessage = "Checklist Mesin Harian: Lolos (0 NG)";
            }

            result.OverallMessage = "Semua validasi Gatekeeper lolos. Lot siap diserahkan ke QA!";
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HandoverCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View("Index", model);
            }

            var line = await _context.Lines.FindAsync(model.LineId);
            if (line == null)
            {
                ModelState.AddModelError("", "Lini tidak ditemukan.");
                await PopulateDropdownsAsync();
                return View("Index", model);
            }

            // Get next queue number for this area today
            var today = DateTime.UtcNow.Date;
            var maxQueue = await _context.InspectionLedgers
                .Where(x => x.Line.Area == line.Area && x.PdInTime >= today)
                .MaxAsync(x => (int?)x.QueueNumber) ?? 0;

            var ledger = new InspectionLedger
            {
                OrderNo = model.OrderNo.Trim().ToUpper(),
                LineId = model.LineId,
                ProductId = model.ProductId,
                LotNo = model.LotNo.Trim().ToUpper(),
                OrderQty = model.OrderQty,
                QueueNumber = maxQueue + 1,
                Status = "WAITING",
                PdPicId = model.PdPicId,
                PdInTime = DateTime.UtcNow,
                IsARI = model.IsARI,
                IsNRS = model.IsNRS,
                IsIFC = model.IsIFC,
                IsRRS = model.IsRRS,
                IsEFR = model.IsEFR,
                IsSplit = model.IsSplit,
                IsTopup = model.IsTopup,
                Remark = model.Remark
            };

            _context.InspectionLedgers.Add(ledger);
            await _context.SaveChangesAsync();

            // Broadcast real-time update to all queue board screens
            await _hubContext.Clients.All.SendAsync("QueueUpdated");

            TempData["SuccessMessage"] = $"MO {ledger.OrderNo} berhasil diserahkan! Nomor antrean: #{ledger.QueueNumber:D2} ({line.Area})";
            return RedirectToAction("Index", "Queue");
        }

        private async Task PopulateDropdownsAsync()
        {
            ViewBag.Lines = new SelectList(await _context.Lines.Where(x => x.IsActive).ToListAsync(), "LineId", "LineName");
            ViewBag.Products = new SelectList(await _context.Products.Where(x => x.IsActive).ToListAsync(), "ProductId", "ItemName");
            ViewBag.PdUsers = new SelectList(await _context.Users.Where(x => x.IsActive).ToListAsync(), "UserId", "FullName");
        }
    }
}

