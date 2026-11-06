using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QLedger.Data;
using QLedger.Hubs;
using QLedger.Models.ViewModels;

namespace QLedger.Controllers
{
    public class NrsController : Controller
    {
        private readonly QLedgerDbContext _context;
        private readonly IHubContext<QueueHub> _hubContext;

        public NrsController(QLedgerDbContext context, IHubContext<QueueHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.NrsReports
                .Include(x => x.Ledger).ThenInclude(l => l.Line)
                .Include(x => x.Ledger).ThenInclude(l => l.Product)
                .Include(x => x.PicUser)
                .Include(x => x.ApprovedBy)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "ALL")
            {
                query = query.Where(x => x.Status == status);
            }

            var reports = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
            ViewBag.CurrentStatus = status ?? "ALL";
            return View(reports);
        }

        [HttpGet]
        public async Task<IActionResult> Investigate(long id)
        {
            var report = await _context.NrsReports
                .Include(x => x.Ledger).ThenInclude(l => l.Line)
                .Include(x => x.Ledger).ThenInclude(l => l.Product)
                .Include(x => x.PicUser)
                .FirstOrDefaultAsync(x => x.NrsId == id);

            if (report == null)
            {
                return NotFound();
            }

            var model = new NrsInvestigationViewModel
            {
                NrsId = report.NrsId,
                Report = report,
                WhyOccur1 = report.WhyOccur1 ?? string.Empty,
                WhyOccur2 = report.WhyOccur2,
                WhyOccur3 = report.WhyOccur3,
                WhyOccur4 = report.WhyOccur4,
                WhyOccur5 = report.WhyOccur5,
                WhyOutflow1 = report.WhyOutflow1 ?? string.Empty,
                WhyOutflow2 = report.WhyOutflow2,
                WhyOutflow3 = report.WhyOutflow3,
                TemporaryAction = report.TemporaryAction ?? string.Empty,
                PermanentAction = report.PermanentAction ?? string.Empty,
                TargetDate = report.TargetDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                PicUserId = report.PicUserId ?? 1,
                Disposition = report.Disposition ?? "REWORK"
            };

            ViewBag.Users = new SelectList(await _context.Users.Where(x => x.IsActive).ToListAsync(), "UserId", "FullName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Investigate(NrsInvestigationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Report = await _context.NrsReports
                    .Include(x => x.Ledger).ThenInclude(l => l.Line)
                    .Include(x => x.Ledger).ThenInclude(l => l.Product)
                    .FirstOrDefaultAsync(x => x.NrsId == model.NrsId);

                ViewBag.Users = new SelectList(await _context.Users.Where(x => x.IsActive).ToListAsync(), "UserId", "FullName");
                return View(model);
            }

            var report = await _context.NrsReports.FindAsync(model.NrsId);
            if (report == null)
            {
                return NotFound();
            }

            report.WhyOccur1 = model.WhyOccur1;
            report.WhyOccur2 = model.WhyOccur2;
            report.WhyOccur3 = model.WhyOccur3;
            report.WhyOccur4 = model.WhyOccur4;
            report.WhyOccur5 = model.WhyOccur5;

            report.WhyOutflow1 = model.WhyOutflow1;
            report.WhyOutflow2 = model.WhyOutflow2;
            report.WhyOutflow3 = model.WhyOutflow3;

            report.TemporaryAction = model.TemporaryAction;
            report.PermanentAction = model.PermanentAction;
            report.TargetDate = model.TargetDate;
            report.PicUserId = model.PicUserId;
            report.Disposition = model.Disposition;
            report.Status = "PENDING_APPROVAL";

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("QueueUpdated");

            TempData["SuccessMessage"] = $"Investigasi 5-Why untuk {report.NrsNumber} telah dikirim dan menunggu persetujuan Supervisor!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(NrsApprovalViewModel model)
        {
            var report = await _context.NrsReports
                .Include(x => x.Ledger)
                .FirstOrDefaultAsync(x => x.NrsId == model.NrsId);

            if (report == null)
            {
                return NotFound();
            }

            report.Status = "CLOSED";
            report.ApprovedById = model.SupervisorId;
            report.ApprovalDate = DateTime.UtcNow;
            report.SupervisorNotes = model.SupervisorNotes;

            // If disposition is REWORK, mark lot as available for rework or close
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("QueueUpdated");

            TempData["SuccessMessage"] = $"Tiket {report.NrsNumber} telah DISETUJUI & DITUTUP oleh Supervisor! Disposisi: {report.Disposition}.";
            return RedirectToAction(nameof(Index));
        }
    }
}

