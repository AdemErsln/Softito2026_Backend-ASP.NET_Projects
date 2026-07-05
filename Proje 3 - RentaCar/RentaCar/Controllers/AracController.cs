using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using RentaCar.Data;
using RentACar.Models;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace RentaCar.Controllers
{
    [Authorize]
    public class AracController : Controller
    {
        ApplicationDBContext _dBContext;

        public AracController(ApplicationDBContext dbContext)
        {
            this._dBContext = dbContext;
        }

        public IActionResult Index(string searchString)
        {
            var query = _dBContext.Araclar.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(a => a.Plaka.Contains(searchString) || 
                                         a.Marka.Contains(searchString) || 
                                         a.Model.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            var result = query.ToList();
            return View(result);
        }

        public IActionResult Create()
        {

            return View();

        }

        [HttpPost]
        
        public IActionResult Create(Araclars araclar)
        {
            _dBContext.Araclar.Add(araclar);
            _dBContext.SaveChanges();

            return View("Index");
        }

        public IActionResult Edit(int id)
        {
          var result =   _dBContext.Araclar.FirstOrDefault(x => x.CarId == id);
            return View(result);
        } 
        [HttpPost]
        public IActionResult Edit(int id , Araclars araclar)
        {
            if (id != araclar.CarId)
            {
                return NotFound() ;
            }
            _dBContext.Update(araclar);
            _dBContext.SaveChanges();
            return View("Index");
        }

        public IActionResult Delete(int id)
        {
            // Veritabanından silinecek aracı ID'sine göre buluyoruz
            var arac = _dBContext.Araclar.FirstOrDefault(x => x.CarId == id);

            if (arac != null)
            {
                // Aracı veritabanından kaldır ve değişiklikleri kaydet
                _dBContext.Araclar.Remove(arac);
                _dBContext.SaveChanges();
            }

            // Silme işleminden sonra Index aksiyonunu tetikliyoruz.
            // Böylece güncel liste veritabanından tekrar çekilir ve ekrana basılır.
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Araclar");
                worksheet.Cells[1, 1].Value = "Araç ID";
                worksheet.Cells[1, 2].Value = "Plaka";
                worksheet.Cells[1, 3].Value = "Marka";
                worksheet.Cells[1, 4].Value = "Model";
                worksheet.Cells[1, 5].Value = "Yıl";
                worksheet.Cells[1, 6].Value = "Günlük Ücret";
                worksheet.Cells[1, 7].Value = "Durum";

                var cars = _dBContext.Araclar.ToList();
                for (int i = 0; i < cars.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = cars[i].CarId;
                    worksheet.Cells[i + 2, 2].Value = cars[i].Plaka;
                    worksheet.Cells[i + 2, 3].Value = cars[i].Marka;
                    worksheet.Cells[i + 2, 4].Value = cars[i].Model;
                    worksheet.Cells[i + 2, 5].Value = cars[i].Yil;
                    worksheet.Cells[i + 2, 6].Value = cars[i].GunlukUcret;
                    worksheet.Cells[i + 2, 7].Value = cars[i].Durum;
                }

                worksheet.Cells.AutoFitColumns();
                var fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Araclar.xlsx");
            }
        }

        [HttpGet]
        public IActionResult ExportToPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var cars = _dBContext.Araclar.ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Text("Araç Filo Envanter Raporu")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Lighten2).Padding(5).Text("ID").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Lighten2).Padding(5).Text("Plaka").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Lighten2).Padding(5).Text("Marka").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Lighten2).Padding(5).Text("Model").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Lighten2).Padding(5).Text("Yıl").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Lighten2).Padding(5).Text("Ücret").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Lighten2).Padding(5).Text("Durum").Bold().FontColor(Colors.White);
                            });

                            // Rows
                            foreach (var car in cars)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(car.CarId.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(car.Plaka);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(car.Marka);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(car.Model);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(car.Yil.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{car.GunlukUcret:C2}");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(car.Durum);
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
                return File(stream.ToArray(), "application/pdf", "Araclar.pdf");
            }
        }
    }
}

