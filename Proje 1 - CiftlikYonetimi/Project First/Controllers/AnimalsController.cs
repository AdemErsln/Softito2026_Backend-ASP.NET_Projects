using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_First.Data;
using Project_First.Models;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Project_First.Controllers
{
    public class AnimalsController : Controller
    {
        public IActionResult Index()
        {
            var animals = _context.Animals.Include(a => a.AnimalType).ToList();
            return View(animals);
        }

        private readonly ApplicationDbContext _context;
        public AnimalsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Create()
        {
          
            var animalTypes =  _context.AnimalType.ToList();

         
            ViewBag.AnimalTypesList = new SelectList(animalTypes, "Id", "animal_type");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Animals animal)
        {
            // AnimalType alanının doğrulama hatasını görmezden gelmesini söylüyoruz
            ModelState.Remove("AnimalType");

            if (ModelState.IsValid)
            {
                _context.Add(animal);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            var animalTypes = _context.AnimalType.ToList();
            ViewBag.AnimalTypesList = new SelectList(animalTypes, "Id", "animal_type", animal.AnimalTypeId);
            return View(animal);
        }

        // 1. DÜZENLEME (GET: Animals/Edit/5)
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            // İlgili hayvanı veri tabanından buluyoruz
            var animal = _context.Animals.Find(id);
            if (animal == null) return NotFound();

            // Dropdown listesinin dolu gelmesi ve mevcut türün seçili olması için
            var animalTypes = _context.AnimalType.ToList();
            ViewBag.AnimalTypesList = new SelectList(animalTypes, "Id", "animal_type", animal.AnimalTypeId);

            return View(animal);
        }

        // 2. DÜZENLEME (POST: Animals/Edit/5)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Animals animal)
        {
            if (id != animal.Id) return NotFound();

            // .NET Core'un takıldığı AnimalType ilişki validasyonunu temizliyoruz
            ModelState.Remove("AnimalType");

            if (ModelState.IsValid)
            {
                _context.Update(animal);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Model geçersizse dropdown boş kalmasın diye tekrar dolduruyoruz
            var animalTypes = _context.AnimalType.ToList();
            ViewBag.AnimalTypesList = new SelectList(animalTypes, "Id", "animal_type", animal.AnimalTypeId);

            return View(animal);
        }

        // 3. SİLME (POST: Animals/Delete/5)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var animal = _context.Animals.Find(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // 4. EXCEL EXPORT
        [HttpGet]
        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Hayvanlar");
                worksheet.Cells[1, 1].Value = "Hayvan ID";
                worksheet.Cells[1, 2].Value = "Hayvan Adı";
                worksheet.Cells[1, 3].Value = "Tür";

                var animals = _context.Animals.Include(a => a.AnimalType).ToList();
                for (int i = 0; i < animals.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = animals[i].Id;
                    worksheet.Cells[i + 2, 2].Value = animals[i].Name;
                    worksheet.Cells[i + 2, 3].Value = animals[i].AnimalType?.animal_type;
                }

                worksheet.Cells.AutoFitColumns();
                var fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Hayvanlar.xlsx");
            }
        }

        // 5. PDF EXPORT
        [HttpGet]
        public IActionResult ExportToPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var animals = _context.Animals.Include(a => a.AnimalType).ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Text("Çiftlik Hayvanları Envanter Raporu")
                        .SemiBold().FontSize(18).FontColor(Colors.Green.Darken2);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Green.Lighten2).Padding(5).Text("ID").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Green.Lighten2).Padding(5).Text("Hayvan Adı").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Green.Lighten2).Padding(5).Text("Hayvan Türü").Bold().FontColor(Colors.White);
                            });

                            // Rows
                            foreach (var animal in animals)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(animal.Id.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(animal.Name);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(animal.AnimalType?.animal_type ?? "-");
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
                return File(stream.ToArray(), "application/pdf", "Hayvanlar.pdf");
            }
        }
    }
}
