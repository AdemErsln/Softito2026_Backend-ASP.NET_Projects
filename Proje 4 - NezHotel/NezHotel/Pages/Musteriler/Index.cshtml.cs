using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NezHotel.Pages.Musteriler
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class IndexModel : PageModel
    {
        public List<MusteriGörünüm> MusteriListesi { get; set; } = new List<MusteriGörünüm>();
        public string SearchString { get; set; }

        public void OnGet(string searchString)
        {
            SearchString = searchString;
            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Musteriler";
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        sql += " WHERE AdSoyad LIKE @Search OR Telefon LIKE @Search OR Email LIKE @Search";
                    }
                    sql += " ORDER BY MusteriID DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            cmd.Parameters.AddWithValue("@Search", "%" + searchString + "%");
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MusteriListesi.Add(new MusteriGörünüm
                                {
                                    MusteriID = reader.GetInt32(0).ToString(),
                                    AdSoyad = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Telefon = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Email = reader.IsDBNull(3) ? "" : reader.GetString(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception) { throw; }
        }

        public IActionResult OnGetExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var list = new List<MusteriGörünüm>();
            
            using (SqlConnection connection = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Musteriler ORDER BY MusteriID DESC";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new MusteriGörünüm
                            {
                                MusteriID = reader.GetInt32(0).ToString(),
                                AdSoyad = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Telefon = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Email = reader.IsDBNull(3) ? "" : reader.GetString(3)
                            });
                        }
                    }
                }
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Musteriler");
                worksheet.Cells[1, 1].Value = "Müşteri ID";
                worksheet.Cells[1, 2].Value = "Adı Soyadı";
                worksheet.Cells[1, 3].Value = "Telefon";
                worksheet.Cells[1, 4].Value = "E-Posta";

                for (int i = 0; i < list.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = list[i].MusteriID;
                    worksheet.Cells[i + 2, 2].Value = list[i].AdSoyad;
                    worksheet.Cells[i + 2, 3].Value = list[i].Telefon;
                    worksheet.Cells[i + 2, 4].Value = list[i].Email;
                }

                worksheet.Cells.AutoFitColumns();
                var fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Musteriler.xlsx");
            }
        }

        public IActionResult OnGetExportToPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var list = new List<MusteriGörünüm>();
            
            using (SqlConnection connection = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Musteriler ORDER BY MusteriID DESC";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new MusteriGörünüm
                            {
                                MusteriID = reader.GetInt32(0).ToString(),
                                AdSoyad = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Telefon = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Email = reader.IsDBNull(3) ? "" : reader.GetString(3)
                            });
                        }
                    }
                }
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
                        .Text("NezHotel Müşteri Kayıt Raporu")
                        .SemiBold().FontSize(18).FontColor(Colors.Grey.Darken3);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(4);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Amber.Medium).Padding(5).Text("ID").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Amber.Medium).Padding(5).Text("Ad Soyad").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Amber.Medium).Padding(5).Text("Telefon").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Amber.Medium).Padding(5).Text("E-Posta").Bold().FontColor(Colors.White);
                            });

                            // Rows
                            foreach (var musteri in list)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(musteri.MusteriID);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(musteri.AdSoyad);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(musteri.Telefon);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(musteri.Email);
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
                return File(stream.ToArray(), "application/pdf", "Musteriler.pdf");
            }
        }
    }

    public class MusteriGörünüm
    {
        public string MusteriID { get; set; }
        public string AdSoyad { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
    }
}