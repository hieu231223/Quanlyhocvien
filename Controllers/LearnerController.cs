using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    [Route("Admin/Learner")]
    public class LearnerController : Controller
    {
        private readonly SchoolContext db;
        private const int pageSize = 3; // Kích thước trang

        public LearnerController(SchoolContext context)
        {
            db = context;
        }

        [HttpGet("Filter")]
        public IActionResult LearnerFilter(string keyword, int? mid, int pageIndex = 1)
        {
            var learners = db.Learners.Include(l => l.Major).AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                learners = learners.Where(l => l.FirstMidName.Contains(keyword) || l.LastName.Contains(keyword));
            }

            // Lọc theo ngành
            if (mid.HasValue && mid.Value > 0)
            {
                learners = learners.Where(l => l.MajorID == mid.Value);
            }

            // Đếm tổng số học viên và tính số trang
            int pageSize = 10; // Số học viên trên mỗi trang
            int totalLearners = learners.Count();
            int pageNum = (int)Math.Ceiling(totalLearners / (float)pageSize);
            ViewBag.pageNum = pageNum;

            // Lấy dữ liệu cho trang hiện tại
            var result = learners.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            ViewBag.CurrentPage = pageIndex;
            ViewBag.keyword = keyword; // Để giữ lại từ khóa tìm kiếm
            ViewBag.mid = mid; // Để giữ lại ngành đã lọc
            ViewBag.Majors = db.Majors.ToList(); // Lấy danh sách ngành cho view

            return PartialView("_LearnerListPartial", result); // Trả về partial view với danh sách học viên
        }

        public IActionResult Index(int? mid, int pageIndex = 1)
{
    var learners = db.Learners.Include(m => m.Major).AsQueryable();

    if (mid != null)
    {
        learners = learners.Where(l => l.MajorID == mid);
        ViewBag.mid = mid;
    }
    else
    {
        ViewBag.mid = null;
    }

    // Tính số trang
    int totalLearners = learners.Count();
    int pageNum = (int)Math.Ceiling(totalLearners / (float)pageSize);
    ViewBag.pageNum = pageNum;

    // Lấy dữ liệu cho trang hiện tại
    var result = learners.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
    ViewBag.CurrentPage = pageIndex;

    // Lấy danh sách ngành cho view
    ViewBag.Majors = db.Majors.ToList();

    return View(result);
        }

        [HttpGet("ByMajor/{mid}")]
        public IActionResult LearnerByMajorID(int mid)
        {
            var learners = db.Learners
                .Where(l => l.MajorID == mid)
                .Include(m => m.Major).ToList();
            return PartialView("LearnerTable", learners);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName");
            return View();
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var learner = db.Learners.Find(id);
            if (learner == null)
            {
                return NotFound();
            }
            ViewBag.MajorId = new SelectList(db.Majors, "MajorID", "MajorName", learner.MajorID);
            return View(learner);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("LearnerID,FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
        {
            if (id != learner.LearnerID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(learner);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearnerExists(learner.LearnerID))
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
            ViewBag.MajorId = new SelectList(db.Majors, "MajorID", "MajorName", learner.MajorID);
            return View(learner);
        }

        private bool LearnerExists(int id)
        {
            return db.Learners.Any(e => e.LearnerID == id);
        }

        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var learner = db.Learners.Include(l => l.Major)
                .Include(e => e.Enrollments)
                .FirstOrDefault(m => m.LearnerID == id);

            if (learner == null)
            {
                return NotFound();
            }

            if (learner.Enrollments.Count() > 0)
            {
                return Content("This learner has some enrollments, can't delete!");
            }

            return View(learner);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var learner = db.Learners.Find(id);
            if (learner != null)
            {
                db.Learners.Remove(learner);
                db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
        {
            if (ModelState.IsValid)
            {
                db.Learners.Add(learner);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName");
            return View();
        }
    }
}