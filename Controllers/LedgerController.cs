using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLedger.Data;
using QLedger.Models.ViewModels;

namespace QLedger.Controllers
{
    public class LedgerController : Controller
    {
        private readonly QLedgerDbContext _context;

        public LedgerController(QLedgerDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(LedgerFilterViewModel filter)
        {
            await PopulateLinesDropdownAsync();
            var query = BuildLedgerQuery(filter);
            filter.Items = await query.OrderByDescending(x => x.PdInTime).ToListAsync();
            return View(filter);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(LedgerFilterViewModel filter)
        {
            var query = BuildLedgerQuery(filter);
            var data = await query.OrderByDescending(x => x.PdInTime).ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("QA Inspection Ledger");

            // Header Row
            var headers = new[]
            {
                "No", "No MO", "Lini", "Area", "Kode Produk", "Nama Produk", "Lot No", "Qty (pcs)",
                "Status", "Judgement", "Dokumen", "PIC Produksi", "Waktu Serah Terima",
                "Inspector QA", "Level Inspeksi", "Waktu Mulai", "Waktu Selesai", "Durasi (menit)", "Catatan"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(20, 50, 100);
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int row = 2;
            int no = 1;
            foreach (var item in data)
            {
                var docs = new List<string>();
                if (item.IsARI) docs.Add("ARI");
                if (item.IsNRS) docs.Add("NRS");
                if (item.IsIFC) docs.Add("IFC");
                if (item.IsRRS) docs.Add("RRS");
                if (item.IsEFR) docs.Add("EFR");
                if (item.IsSplit) docs.Add("SPLIT");
                if (item.IsTopup) docs.Add("TOPUP");

                ws.Cell(row, 1).Value = no++;
                ws.Cell(row, 2).Value = item.OrderNo;
                ws.Cell(row, 3).Value = item.Line?.LineName ?? "-";
                ws.Cell(row, 4).Value = item.Line?.Area ?? "-";
                ws.Cell(row, 5).Value = item.Product?.ItemCode ?? "-";
                ws.Cell(row, 6).Value = item.Product?.ItemName ?? "-";
                ws.Cell(row, 7).Value = item.LotNo;
                ws.Cell(row, 8).Value = item.OrderQty;
                ws.Cell(row, 9).Value = item.Status;
                ws.Cell(row, 10).Value = item.QaJudgement ?? "-";
                ws.Cell(row, 11).Value = string.Join(", ", docs);
                ws.Cell(row, 12).Value = item.PdPic?.FullName ?? "-";
                ws.Cell(row, 13).Value = item.PdInTime.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Cell(row, 14).Value = item.QaInspector?.FullName ?? "-";
                ws.Cell(row, 15).Value = item.InspectionLevel ?? "-";
                ws.Cell(row, 16).Value = item.QaStartTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-";
                ws.Cell(row, 17).Value = item.QaFinishTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-";
                if (item.InspectionDurationMinutes.HasValue)
                {
                    ws.Cell(row, 18).Value = Math.Round(item.InspectionDurationMinutes.Value, 1);
                }
                else
                {
                    ws.Cell(row, 18).Value = "-";
                }
                ws.Cell(row, 19).Value = item.Remark ?? "-";

                // Highlight Judgement column
                if (item.QaJudgement == "OK")
                {
                    ws.Cell(row, 10).Style.Font.FontColor = XLColor.Green;
                    ws.Cell(row, 10).Style.Font.Bold = true;
                }
                else if (item.QaJudgement == "NG")
                {
                    ws.Cell(row, 10).Style.Font.FontColor = XLColor.Red;
                    ws.Cell(row, 10).Style.Font.Bold = true;
                }

                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            var fileName = $"QA_Inspection_Ledger_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private IQueryable<Models.Entities.InspectionLedger> BuildLedgerQuery(LedgerFilterViewModel filter)
        {
            var query = _context.InspectionLedgers
                .Include(x => x.Line)
                .Include(x => x.Product)
                .Include(x => x.PdPic)
                .Include(x => x.QaInspector)
                .AsQueryable();

            if (filter.StartDate.HasValue)
            {
                var start = filter.StartDate.Value.Date;
                query = query.Where(x => x.PdInTime >= start);
            }

            if (filter.EndDate.HasValue)
            {
                var end = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.PdInTime <= end);
            }

            if (filter.LineId.HasValue && filter.LineId.Value > 0)
            {
                query = query.Where(x => x.LineId == filter.LineId.Value);
            }

            if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "ALL")
            {
                query = query.Where(x => x.Status == filter.Status);
            }

            if (!string.IsNullOrEmpty(filter.Judgement) && filter.Judgement != "ALL")
            {
                query = query.Where(x => x.QaJudgement == filter.Judgement);
            }

            if (!string.IsNullOrEmpty(filter.TagFilter) && filter.TagFilter != "ALL")
            {
                switch (filter.TagFilter)
                {
                    case "ARI": query = query.Where(x => x.IsARI); break;
                    case "NRS": query = query.Where(x => x.IsNRS); break;
                    case "IFC": query = query.Where(x => x.IsIFC); break;
                    case "RRS": query = query.Where(x => x.IsRRS); break;
                    case "EFR": query = query.Where(x => x.IsEFR); break;
                    case "SPLIT": query = query.Where(x => x.IsSplit); break;
                    case "TOPUP": query = query.Where(x => x.IsTopup); break;
                }
            }

            return query;
        }

        private async Task PopulateLinesDropdownAsync()
        {
            ViewBag.Lines = new SelectList(await _context.Lines.Where(x => x.IsActive).ToListAsync(), "LineId", "LineName");
        }
    }
}
