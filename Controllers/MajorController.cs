using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApp.Controllers
{
    [Route("Admin/Major")]
    public class MajorController : Controller
    {
        private readonly SchoolContext _context;
        private const int pageSize = 3;

        public MajorController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Admin/Major
        [HttpGet]
        public async Task<IActionResult> Index(int? pageIndex = 1, string? keyword = null)
        {
            var majors = _context.Majors.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                majors = majors.Where(m => m.MajorName.ToLower().Contains(keyword.ToLower()));
            }

            int totalCount = await majors.CountAsync();
            int pageNum = (int)Math.Ceiling(totalCount / (float)pageSize);
            ViewBag.pageNum = pageNum;
            ViewBag.CurrentPage = pageIndex; // Cập nhật CurrentPage
            ViewBag.keyword = keyword; // Để giữ lại từ khóa tìm kiếm

            var result = await majors.OrderBy(m => m.MajorName)
                .Skip(pageSize * ((pageIndex ?? 1) - 1))
                .Take(pageSize)
                .ToListAsync();

            return View(result);
        }

        // GET: Admin/Major/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Major/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Major major)
        {
            if (ModelState.IsValid)
            {
                _context.Majors.Add(major);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Chuyển hướng về danh sách ngành
            }
            return View(major);
        }

        // GET: Admin/Major/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var major = await _context.Majors.FindAsync(id);
            if (major == null)
            {
                return NotFound();
            }
            return View(major);
        }

        // POST: Admin/Major/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Major major)
        {
            if (id != major.MajorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(major);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MajorExists(major.MajorID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(major);
        }

        // GET: Admin/Major/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var major = await _context.Majors.FindAsync(id);
            if (major == null)
            {
                return NotFound();
            }
            return View(major);
        }

        // POST: Admin/Major/DeleteConfirmed/5
        [HttpPost("DeleteConfirmed/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var major = await _context.Majors.FindAsync(id);
            if (major != null)
            {
                _context.Majors.Remove(major);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Kiểm tra sự tồn tại của Major
        private bool MajorExists(int id)
        {
            return _context.Majors.Any(e => e.MajorID == id);
        }
    }
}