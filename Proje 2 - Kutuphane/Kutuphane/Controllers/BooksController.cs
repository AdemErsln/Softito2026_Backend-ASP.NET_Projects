using Kutuphane.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Kutuphane.Controllers
{
    [Authorize]
    public class Books : Controller
    {
        private readonly AppDbContext _context;

        public Books(AppDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME VE ARAMA
        public IActionResult Index(string searchString)
        {
            var query = _context.Books.Include(b => b.Author).AsQueryable();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.Title.Contains(searchString) || b.Author.Name.Contains(searchString));
                ViewBag.SearchString = searchString;
            }
            
            return View(query.ToList());
        }

        // 2. EKLEME (GET)
        public IActionResult Create()
        {
            // Kitap eklerken yazar seçebilmek için yazarları SelectList olarak View'a gönderiyoruz
            ViewBag.AuthorId = new SelectList(_context.Authors, "AuthorId", "Name");
            return View();
        }

        // 3. EKLEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"'{book.Title}' isimli kitap başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AuthorId = new SelectList(_context.Authors, "AuthorId", "Name", book.AuthorId);
            return View(book);
        }

        // 4. GÜNCELLEME (GET)
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();

            ViewBag.AuthorId = new SelectList(_context.Authors, "AuthorId", "Name", book.AuthorId);
            return View(book);
        }

        // 5. GÜNCELLEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Book book)
        {
            if (id != book.BookId) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(book);
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"'{book.Title}' başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AuthorId = new SelectList(_context.Authors, "AuthorId", "Name", book.AuthorId);
            return View(book);
        }

        // 6. SİLME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kitap başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        // 7. EXCEL EXPORT
        [HttpGet]
        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Kitaplar");
                worksheet.Cells[1, 1].Value = "Kitap ID";
                worksheet.Cells[1, 2].Value = "Kitap Adı";
                worksheet.Cells[1, 3].Value = "Yayın Yılı";
                worksheet.Cells[1, 4].Value = "Yazar";

                var books = _context.Books.Include(b => b.Author).ToList();
                for (int i = 0; i < books.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = books[i].BookId;
                    worksheet.Cells[i + 2, 2].Value = books[i].Title;
                    worksheet.Cells[i + 2, 3].Value = books[i].PublishYear;
                    worksheet.Cells[i + 2, 4].Value = books[i].Author?.Name;
                }

                worksheet.Cells.AutoFitColumns();
                var fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kitaplar.xlsx");
            }
        }

        // 8. PDF EXPORT
        [HttpGet]
        public IActionResult ExportToPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var books = _context.Books.Include(b => b.Author).ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Text("Kütüphane Kitap Envanter Raporu")
                        .SemiBold().FontSize(18).FontColor(Colors.Purple.Darken2);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Purple.Lighten2).Padding(5).Text("ID").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Purple.Lighten2).Padding(5).Text("Kitap Adı").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Purple.Lighten2).Padding(5).Text("Yayın Yılı").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Purple.Lighten2).Padding(5).Text("Yazar").Bold().FontColor(Colors.White);
                            });

                            // Rows
                            foreach (var book in books)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(book.BookId.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(book.Title);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(book.PublishYear.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(book.Author?.Name ?? "-");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Sayfa ");
                            x.CurrentPageNumber();
                        });
                });
            });

            using (var stream = new System.IO.MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", "Kitaplar.pdf");
            }
        }
    }
}