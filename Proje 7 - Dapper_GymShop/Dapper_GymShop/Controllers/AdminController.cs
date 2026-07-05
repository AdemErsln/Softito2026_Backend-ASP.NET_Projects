using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper_GymShop.Models;
using Dapper;
using Dapper_GymShop.Models.ShippingCompany;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace Dapper_GymShop.Controllers
{
    public class AdminController : Controller
    {
        // ==================== GİRİŞ ====================

       public IActionResult Login(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                // SQL'deki sp_AdminLogin prosedürünü çalıştırıp eşleşen admini buluyoruz
                var admin = connection.QueryFirstOrDefault<Admin>(
                    "sp_AdminLogin",
                    new { Username = username, Password = password },
                    commandType: CommandType.StoredProcedure
                );

                // Eğer veritabanından bir kayıt döndüyse giriş başarılıdır
                if (admin != null)
                {
                    // Giriş başarılı: Yönetim paneline (Dashboard) yönlendiriyoruz
                    return RedirectToAction("Dashboard");
                }

                // Eğer kayıt gelmediyse şifre veya kullanıcı adı hatalıdır
                ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
                return View(); // Aynı sayfayı hata mesajıyla birlikte tekrar yükler
            }
        }

        // ==================== DASHBOARD ====================

        // GET: /Admin/Dashboard
        // Giriş yapıldıktan sonra ulaşılan panel ekranı
        [HttpGet]
        public IActionResult Dashboard()
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                // Panelde listelemek için ürünleri ve siparişleri çekiyoruz
                var products = connection.Query<Product>("sp_GetIndexProducts", commandType: CommandType.StoredProcedure).ToList();
                var orders = connection.Query<Order>("sp_GetOrders", commandType: CommandType.StoredProcedure).ToList();

                ViewBag.Orders = orders; // Siparişleri ViewBag ile gönderdik
                return View(products);  // Ürünleri ana model (@model IEnumerable<Product>) olarak gönderdik
            }
        }

        // ==================== ÜRÜN YÖNETİMİ ====================

        public IActionResult Products()
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var products = connection.Query<Product>("sp_GetIndexProducts", commandType: CommandType.StoredProcedure).ToList();
                return View(products);
            }
        }

        [HttpPost]
        public IActionResult AddProduct(string ProductName, decimal Price, int Stock, string ImageURL)
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var param = new DynamicParameters();
                param.Add("@ProductName", ProductName);
                param.Add("@Price", Price);
                param.Add("@Stock", Stock);
                param.Add("@ImageURL", ImageURL);
                connection.Execute("sp_AddProduct", param, commandType: CommandType.StoredProcedure);
            }
            return RedirectToAction("Products");
        }

        [HttpPost]
        public IActionResult UpdateProduct(int ID, string ProductName, decimal Price, int Stock, string ImageURL)
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var param = new DynamicParameters();
                param.Add("@ID", ID);
                param.Add("@ProductName", ProductName);
                param.Add("@Price", Price);
                param.Add("@Stock", Stock);
                param.Add("@ImageURL", ImageURL);
                connection.Execute("sp_UpdateProduct", param, commandType: CommandType.StoredProcedure);
            }
            return RedirectToAction("Products");
        }

        public IActionResult DeleteProduct(int id)
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var param = new DynamicParameters();
                param.Add("@ID", id);
                connection.Execute("sp_DeleteProduct", param, commandType: CommandType.StoredProcedure);
            }
            return RedirectToAction("Products");
        }

        // ==================== SİPARİŞ YÖNETİMİ ====================

        public IActionResult Orders()
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                // FIX: Query<Product> yerine Query<Order> kullanılmalı
                var orders = connection.Query<Order>("sp_GetOrders", commandType: CommandType.StoredProcedure).ToList();
                return View(orders);
            }
        }

        // ==================== KARGO YÖNETİMİ ====================

        public IActionResult Shipping()
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                // FIX: Boş view dönmek yerine veritabanından kargo firmalarını çekiyoruz
                var shippings = connection.Query<ShippingCompany>("sp_GetShippingCompanies", commandType: CommandType.StoredProcedure).ToList();
                return View(shippings);
            }
        }

        [HttpPost]
        public IActionResult AddShipping(string CompanyName, decimal ShippingPrice, int EstimatedDeliveryDays)
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var param = new DynamicParameters();
                param.Add("@CompanyName", CompanyName);
                param.Add("@ShippingPrice", ShippingPrice);
                param.Add("@EstimatedDeliveryDays", EstimatedDeliveryDays);
                connection.Execute("sp_AddShipping", param, commandType: CommandType.StoredProcedure);
            }
            return RedirectToAction("Shipping");
        }

        [HttpPost]
        public IActionResult UpdateShipping(int ID, string CompanyName, decimal ShippingPrice, int EstimatedDeliveryDays)
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var param = new DynamicParameters();
                param.Add("@ID", ID);
                param.Add("@CompanyName", CompanyName);
                param.Add("@ShippingPrice", ShippingPrice);
                param.Add("@EstimatedDeliveryDays", EstimatedDeliveryDays);
                connection.Execute("sp_UpdateShipping", param, commandType: CommandType.StoredProcedure);
            }
            return RedirectToAction("Shipping");
        }

        public IActionResult DeleteShipping(int id)
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var param = new DynamicParameters();
                param.Add("@ID", id);
                connection.Execute("sp_DeleteShipping", param, commandType: CommandType.StoredProcedure);
            }
            return RedirectToAction("Shipping");
        }

        // ==================== YÖNETİCİ (ADMİN) YÖNETİMİ ====================

        public IActionResult Users()
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                // FIX: Query<Product> yerine Query<Admin> kullanılmalı
                var admins = connection.Query<Admin>("sp_GetAdmins", commandType: CommandType.StoredProcedure).ToList();
                return View(admins);
            }
        }

        [HttpPost]
        public IActionResult AddAdmin(string Username, string Password)
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var param = new DynamicParameters();
                param.Add("@Username", Username);
                param.Add("@Password", Password);
                connection.Execute("sp_AddAdmin", param, commandType: CommandType.StoredProcedure);
            }
            return RedirectToAction("Users");
        }

        public IActionResult DeleteAdmin(int id)
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var param = new DynamicParameters();
                param.Add("@ID", id);
                connection.Execute("sp_DeleteAdmin", param, commandType: CommandType.StoredProcedure);
            }
            return RedirectToAction("Users");
        }

        // ==================== TEST & INDEX ====================

        public IActionResult Test()
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var products = connection.Query<Product>("sp_GetIndexProducts", commandType: CommandType.StoredProcedure).ToList();
                var shippings = connection.Query<ShippingCompany>("sp_GetShippingCompanies", commandType: CommandType.StoredProcedure).ToList();

                ViewBag.Shippings = shippings;
                return View(products); // Ürün listesini ana model olarak gönderiyoruz
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ExportProductsToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<Product> products;
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                products = connection.Query<Product>("sp_GetIndexProducts", commandType: CommandType.StoredProcedure).ToList();
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Urunler");
                worksheet.Cells[1, 1].Value = "Ürün ID";
                worksheet.Cells[1, 2].Value = "Ürün Adı";
                worksheet.Cells[1, 3].Value = "Fiyat";
                worksheet.Cells[1, 4].Value = "Stok";

                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = products[i].ID;
                    worksheet.Cells[i + 2, 2].Value = products[i].ProductName;
                    worksheet.Cells[i + 2, 3].Value = products[i].Price;
                    worksheet.Cells[i + 2, 4].Value = products[i].Stock;
                }

                worksheet.Cells.AutoFitColumns();
                var fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Urunler.xlsx");
            }
        }

        [HttpGet]
        public IActionResult ExportProductsToPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            List<Product> products;
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                products = connection.Query<Product>("sp_GetIndexProducts", commandType: CommandType.StoredProcedure).ToList();
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Text("GymShop Ürün Envanter Raporu")
                        .SemiBold().FontSize(18).FontColor(Colors.Green.Darken3);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Green.Medium).Padding(5).Text("ID").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Ürün Adı").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Fiyat").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Stok").Bold().FontColor(Colors.White);
                            });

                            // Rows
                            foreach (var p in products)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.ID.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.ProductName ?? "");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Price.ToString("N2") + " TL");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Stock.ToString());
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
                return File(stream.ToArray(), "application/pdf", "Urunler.pdf");
            }
        }
    }
}
