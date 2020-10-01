using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CGS_CIMB_TechnicalTest.Data;
using CGS_CIMB_TechnicalTest.Models;
using System.IO;

namespace CGS_CIMB_TechnicalTest.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        const String folderName = @"files\";
        readonly String folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Reports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reports>>> GetReports()
        {
            return await _context.Reports.ToListAsync();
        }

        // GET: api/Reports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reports>> GetReports(Guid id)
        {
            //var reports = await _context.ReportFiles.FindAsync(id);

            //if (reports == null)
            //{
            //    return NotFound();
            //}

            //var filename = _context.ReportFiles.Where(x => x.Id == id).Select(x => x.FileName).SingleOrDefault();

            //if (filename == null)
            //    return Content("filename not present");

            //var folder = _context.FileUpload.Where(x => x.FileFolder == filename).Select(x => x.FilePath).SingleOrDefault();

            var path = _context.Reports.Include(x => x.ReportFiles).Where(x => x.Id == id).Select(x => x.ReportFiles.FilePath).SingleOrDefault();

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            Reports reports = _context.Reports.Where(x => x.Id == id).SingleOrDefault();

            reports.ReadershipStats = reports.ReadershipStats + 1;
            _context.Reports.Update(reports);
            await _context.SaveChangesAsync();

            return File(memory, GetContentType(path), path);
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".zip", "application/zip"},
                {".rar", "application/rar"}
            };
        }

        // PUT: api/Reports/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReports(Guid id, Reports reports)
        {
            if (id != reports.Id)
            {
                return BadRequest();
            }

            _context.Entry(reports).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Reports
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Reports>> PostReports(IFormFile files, string title)
        {
            ReportFiles reportFiles = new ReportFiles();
            Reports reports = new Reports();

            using (var fileContentStream = new MemoryStream())
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                await files.CopyToAsync(fileContentStream);
                await System.IO.File.WriteAllBytesAsync(Path.Combine(folderPath, files.FileName), fileContentStream.ToArray());
            }

            reportFiles.Id = Guid.NewGuid();
            reportFiles.FileName = files.FileName;
            reportFiles.FilePath = Path.GetFullPath(folderPath + files.FileName);
            reportFiles.FileExt = Path.GetExtension(files.FileName);
            _context.ReportFiles.Add(reportFiles);
            await _context.SaveChangesAsync();

            //_context.ReportFiles.Add(reports);
            //await _context.SaveChangesAsync();

            //return CreatedAtRoute(routeName: "myFile", routeValues: new { filename = files.FileName }, value: null);
            reports.Id = Guid.NewGuid();
            reports.Title = title;
            reports.ReportFilesId = reportFiles.Id;
            _context.Reports.Add(reports);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReports", new { id = reports.Id }, reports);
        }

        // DELETE: api/Reports/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Reports>> DeleteReports(Guid id)
        {
            var reports = await _context.Reports.FindAsync(id);
            if (reports == null)
            {
                return NotFound();
            }

            _context.Reports.Remove(reports);
            await _context.SaveChangesAsync();

            return reports;
        }

        private bool ReportsExists(Guid id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}
