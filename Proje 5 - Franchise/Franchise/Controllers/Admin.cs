using Franchise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace Franchise.Controllers
{
    public class Admin : Controller
    {

        ApplicationDbContext _dbcontext;

        public Admin(ApplicationDbContext dbContext) {
            _dbcontext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Buyers( ) {
            return View();
        }

        [HttpGet]
        public IActionResult GetBuyers()
        {
            // .Select kullanarak veriyi JS'in tam istediği formata (bütünleşik isimlerle) çeviriyoruz
            var buyers = _dbcontext.FranchiseBuyers
                .Select(b => new {
                    buyerID = b.BuyerID,
                    buyerName = b.BuyerName,
                    notes = b.Notes,
                    // Arka planda paketi bulup ismini çekiyoruz (Include yazmaya gerek kalmaz)
                    packageName = _dbcontext.FranchisePackages
                                            .Where(p => p.PackageID == b.PackageID)
                                            .Select(p => p.PackageName)
                                            .FirstOrDefault()
                }).ToList();

            // Veriyi direkt dizi olarak dönüyoruz
            return Json(buyers);
        }

        [HttpPost]
        public JsonResult AddBuyer(FranchiseBuyer buyer)
        {
            _dbcontext.Add(buyer);
            _dbcontext.SaveChanges();

            return Json(new { success = true, message = "Müşeteri Başarıyla Eklendi" })
;        }


     
        public JsonResult GetBuyerById(int id)
        {
            var user = _dbcontext.FranchiseBuyers.Find(id);
            return Json(user);
        }

        public JsonResult UpdateBuyer(FranchiseBuyer buyer )
        {
            _dbcontext.Update(buyer);
            _dbcontext.SaveChanges();
            return Json(new { success = true, message = "Müşeteri Başarıyla Güncellendi" });

        }
        [HttpPost]
        public JsonResult DeleteBuyer(int id)
        {
            try
            {            
                
                var buyer = _dbcontext.FranchiseBuyers.Find(id);
                _dbcontext.FranchiseBuyers.Remove(buyer);
                _dbcontext.SaveChanges();


                return Json(new { success = true, message = $"Müşteri başarıyla silindi" });


            }
            catch (Exception)
            {

                throw;
            }
        }



        public IActionResult Packages()
        {
            return View();
        }

        // ==========================================
        // 1. TÜM PAKETLERİ LİSTELE (GET)
        // ==========================================
        [HttpGet]
        public IActionResult GetPackages()
        {
            try
            {
                var packages = _dbcontext.FranchisePackages
                    .Select(p => new {
                        packageID = p.PackageID,
                        packageName = p.PackageName,
                        price = p.Price // Modelindeki isimle tam uyuştu
                    }).ToList();

                return Json(packages);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 2. TEK BİR PAKETİ GETİR (GET - Düzenleme İçin)
        // ==========================================
        [HttpGet]
        public IActionResult GetPackageById(int id)
        {
            try
            {
                var pack = _dbcontext.FranchisePackages
                    .Select(p => new {
                        packageID = p.PackageID,
                        packageName = p.PackageName,
                        price = p.Price
                    })
                    .FirstOrDefault(p => p.packageID == id);

                return Json(pack);
            }
            catch
            {
                return Json(null);
            }
        }

        // ==========================================
        // 3. YENİ PAKET EKLE (POST)
        // ==========================================
        [HttpPost]
        public IActionResult AddPackage(FranchisePackage model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.PackageName))
                {
                    return Json(new { success = false, message = "Paket adı boş geçilemez." });
                }

                _dbcontext.FranchisePackages.Add(model);
                _dbcontext.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 4. PAKETİ GÜNCELLE (POST)
        // ==========================================
        [HttpPost]
        public IActionResult UpdatePackage(FranchisePackage model)
        {
            try
            {
                var existingPackage = _dbcontext.FranchisePackages.Find(model.PackageID);
                if (existingPackage == null)
                {
                    return Json(new { success = false, message = "Paket bulunamadı." });
                }

                existingPackage.PackageName = model.PackageName;
                existingPackage.Price = model.Price; // Güncelleme alanını bağladık

                _dbcontext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 5. PAKETİ SİL (POST)
        // ==========================================
        [HttpPost]
        public IActionResult DeletePackage(int id)
        {
            try
            {
                var pack = _dbcontext.FranchisePackages.Find(id);
                if (pack == null)
                {
                    return Json(new { success = false, message = "Silinecek paket bulunamadı." });
                }

                // İlişkili veri güvenliği: Silinen pakete bağlı alıcılar varsa patlamaması için packageID'lerini boşa çıkarıyoruz
                var linkedBuyers = _dbcontext.FranchiseBuyers.Where(b => b.PackageID == id).ToList();
                foreach (var buyer in linkedBuyers)
                {
                    buyer.PackageID = null;
                }

                _dbcontext.FranchisePackages.Remove(pack);
                _dbcontext.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Paket silinirken hata: {ex.Message}" });
            }
        }

        // ==================== RAPORLAMA ====================

        public IActionResult Report()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetReportData()
        {
            try
            {
                var totalBuyers = _dbcontext.FranchiseBuyers.Count();
                var totalPackages = _dbcontext.FranchisePackages.Count();
                var buyersWithPackage = _dbcontext.FranchiseBuyers.Count(b => b.PackageID != null);
                var buyersWithoutPackage = totalBuyers - buyersWithPackage;

                // Paket bazında alıcı dağılımı
                var packageDistribution = _dbcontext.FranchisePackages
                    .Select(p => new
                    {
                        packageName = p.PackageName,
                        price = p.Price,
                        buyerCount = _dbcontext.FranchiseBuyers.Count(b => b.PackageID == p.PackageID)
                    }).ToList();

                // En son eklenen 10 alıcı
                var recentBuyers = _dbcontext.FranchiseBuyers
                    .OrderByDescending(b => b.BuyerID)
                    .Take(10)
                    .Select(b => new
                    {
                        buyerName = b.BuyerName,
                        notes = b.Notes,
                        packageName = _dbcontext.FranchisePackages
                            .Where(p => p.PackageID == b.PackageID)
                            .Select(p => p.PackageName)
                            .FirstOrDefault() ?? "Paket Yok"
                    }).ToList();

                return Json(new
                {
                    success = true,
                    totalBuyers,
                    totalPackages,
                    buyersWithPackage,
                    buyersWithoutPackage,
                    packageDistribution,
                    recentBuyers
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ExportBuyersToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var buyers = _dbcontext.FranchiseBuyers
                .Include(b => b.FranchisePackage)
                .ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Franchise Musterileri");
                worksheet.Cells[1, 1].Value = "Alıcı ID";
                worksheet.Cells[1, 2].Value = "Alıcı Adı / Ticari Unvanı";
                worksheet.Cells[1, 3].Value = "Tercih Ettiği Paket";
                worksheet.Cells[1, 4].Value = "Notlar";

                for (int i = 0; i < buyers.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = buyers[i].BuyerID;
                    worksheet.Cells[i + 2, 2].Value = buyers[i].BuyerName;
                    worksheet.Cells[i + 2, 3].Value = buyers[i].FranchisePackage?.PackageName ?? "Seçim Yapılmadı";
                    worksheet.Cells[i + 2, 4].Value = buyers[i].Notes;
                }

                worksheet.Cells.AutoFitColumns();
                var fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FranchiseMusterileri.xlsx");
            }
        }

        [HttpGet]
        public IActionResult ExportBuyersToPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var buyers = _dbcontext.FranchiseBuyers
                .Include(b => b.FranchisePackage)
                .ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Text("Franchise Alıcı Kayıt Raporu")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken3);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(4);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("ID").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Alıcı Adı").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Tercih Ettiği Paket").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Notlar").Bold().FontColor(Colors.White);
                            });

                            // Rows
                            foreach (var buyer in buyers)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(buyer.BuyerID.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(buyer.BuyerName);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(buyer.FranchisePackage?.PackageName ?? "Paket Yok");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(buyer.Notes);
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

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", "FranchiseMusterileri.pdf");
            }
        }
    }
}
