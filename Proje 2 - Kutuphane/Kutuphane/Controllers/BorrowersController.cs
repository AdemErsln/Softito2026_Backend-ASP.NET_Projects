using Kutuphane.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane.Controllers
{
    public class Borrowers : Controller
    {
        private readonly AppDbContext _context;

        public Borrowers(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var borrowers = _context.Borrowers.Include(b => b.Book).ToList();
            return View(borrowers);
        }

        public IActionResult Create()
        {
            ViewBag.BookId = new SelectList(_context.Books, "BookId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Borrower borrower)
        {
            if (ModelState.IsValid)
            {
                _context.Borrowers.Add(borrower);
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"{borrower.FullName} için ödünç kaydı açıldı.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BookId = new SelectList(_context.Books, "BookId", "Title", borrower.BookId);
            return View(borrower);
        }

        public IActionResult Edit(int id)
        {
            var borrower = _context.Borrowers.Find(id);
            if (borrower == null) return NotFound();

            ViewBag.BookId = new SelectList(_context.Books, "BookId", "Title", borrower.BookId);
            return View(borrower);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Borrower borrower)
        {
            if (id != borrower.BorrowerId && id != borrower.BorrowerId) return BadRequest(); // Küçük typo ihtimaline karşı ikisini de yazdım

            if (ModelState.IsValid)
            {
                _context.Update(borrower);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Ödünç kaydı güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BookId = new SelectList(_context.Books, "BookId", "Title", borrower.BookId);
            return View(borrower);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var borrower = _context.Borrowers.Find(id);
            if (borrower != null)
            {
                _context.Borrowers.Remove(borrower);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Ödünç kaydı başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}