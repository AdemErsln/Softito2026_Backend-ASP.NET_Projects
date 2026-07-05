using Kutuphane.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalBooks = _context.Books.Count();
            ViewBag.TotalAuthors = _context.Authors.Count();
            ViewBag.TotalBorrowers = _context.Borrowers.Count();

            // 2. Grafik Raporu 1: En çok kitabı olan top 5 yazar (Grafik Verisi)
            var topAuthors = _context.Authors
                .Select(a => new { Name = a.Name, BookCount = _context.Books.Count(b => b.AuthorId == a.AuthorId) })
                .OrderByDescending(x => x.BookCount)
                .Take(5)
                .ToList();

            ViewBag.AuthorLabels = string.Join(",", topAuthors.Select(x => $"'{x.Name}'"));
            ViewBag.AuthorData = string.Join(",", topAuthors.Select(x => x.BookCount));

            return View();
        }


        
    }
}
