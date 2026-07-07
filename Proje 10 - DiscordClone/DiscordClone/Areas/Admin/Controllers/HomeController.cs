using DiscordClone.Data.Repository.IRepository;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Linq;

namespace DiscordClone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: /Admin/Home/Index
        public IActionResult Index()
        {
            ViewBag.UserCount = _userManager.Users.Count();
            ViewBag.ServerCount = _unitOfWork.Server.GetAll().Count();
            ViewBag.ChannelCount = _unitOfWork.Channel.GetAll().Count();
            ViewBag.MessageCount = _unitOfWork.Message.GetAll().Count();

            var servers = _unitOfWork.Server.GetAll(includeProperties: "Owner").ToList();
            return View(servers);
        }

        // ----------------------------------------------------
        // SERVERS REPORT
        // ----------------------------------------------------
        public IActionResult ExportExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Disaccord Developer");
            var servers = _unitOfWork.Server.GetAll(includeProperties: "Owner").ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Disaccord Sunucuları");
                worksheet.Cells[1, 1].Value = "Sunucu ID";
                worksheet.Cells[1, 2].Value = "Sunucu Adı";
                worksheet.Cells[1, 3].Value = "Davet Kodu";
                worksheet.Cells[1, 4].Value = "Kurucu Adı";
                worksheet.Cells[1, 5].Value = "Kurucu E-postası";

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(88, 101, 242));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var server in servers)
                {
                    worksheet.Cells[row, 1].Value = server.Id;
                    worksheet.Cells[row, 2].Value = server.Name;
                    worksheet.Cells[row, 3].Value = server.InvitationCode;
                    worksheet.Cells[row, 4].Value = server.Owner?.UserName ?? "Bilinmiyor";
                    worksheet.Cells[row, 5].Value = server.Owner?.Email ?? "Bilinmiyor";
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Disaccord_Sunucu_Raporu.xlsx");
            }
        }

        public IActionResult ExportPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var userCount = _userManager.Users.Count();
            var serverCount = _unitOfWork.Server.GetAll().Count();
            var channelCount = _unitOfWork.Channel.GetAll().Count();
            var messageCount = _unitOfWork.Message.GetAll().Count();
            var servers = _unitOfWork.Server.GetAll(includeProperties: "Owner").ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial).FontSize(11));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("DISACCORD RAPORU").FontSize(24).Bold().FontColor("#5865F2");
                            col.Item().Text("Sistem İstatistikleri ve Sunucu Listesi").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                        });
                        row.ConstantItem(50).AlignRight().Text("ADMIN").FontSize(12).Bold().FontColor("#F23F43");
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text("Sistem Özeti").FontSize(14).Bold().Underline();
                        col.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem().Text($"Toplam Kullanıcı: {userCount}");
                            row.RelativeItem().Text($"Toplam Sunucu: {serverCount}");
                            row.RelativeItem().Text($"Toplam Kanal: {channelCount}");
                            row.RelativeItem().Text($"Toplam Mesaj: {messageCount}");
                        });

                        col.Item().PaddingTop(1.5f, Unit.Centimetre).Text("Aktif Sunucu Listesi").FontSize(14).Bold().Underline();

                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#5865F2").Padding(5).Text("ID").Bold().FontColor(Colors.White);
                                header.Cell().Background("#5865F2").Padding(5).Text("Sunucu Adı").Bold().FontColor(Colors.White);
                                header.Cell().Background("#5865F2").Padding(5).Text("Davet Kodu").Bold().FontColor(Colors.White);
                                header.Cell().Background("#5865F2").Padding(5).Text("Kurucu").Bold().FontColor(Colors.White);
                            });

                            foreach (var server in servers)
                            {
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(server.Id.ToString());
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(server.Name);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(server.InvitationCode);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(server.Owner?.UserName ?? "Bilinmiyor");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Sayfa ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", "Disaccord_Sistem_Raporu.pdf");
            }
        }

        // ----------------------------------------------------
        // USERS REPORT
        // ----------------------------------------------------
        public IActionResult ExportUsersExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Disaccord Developer");
            var users = _userManager.Users.ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Disaccord Kullanıcıları");
                worksheet.Cells[1, 1].Value = "Kullanıcı ID";
                worksheet.Cells[1, 2].Value = "Kullanıcı Adı";
                worksheet.Cells[1, 3].Value = "E-posta";
                worksheet.Cells[1, 4].Value = "Durum";
                worksheet.Cells[1, 5].Value = "Biyografi";

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(88, 101, 242));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var u in users)
                {
                    worksheet.Cells[row, 1].Value = u.Id;
                    worksheet.Cells[row, 2].Value = u.UserName;
                    worksheet.Cells[row, 3].Value = u.Email;
                    worksheet.Cells[row, 4].Value = u.Status;
                    worksheet.Cells[row, 5].Value = u.Bio;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Disaccord_Kullanici_Raporu.xlsx");
            }
        }

        public IActionResult ExportUsersPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var users = _userManager.Users.ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial).FontSize(11));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("DISACCORD KULLANICI RAPORU").FontSize(20).Bold().FontColor("#5865F2");
                            col.Item().Text("Sistemdeki Kayıtlı Kullanıcı Listesi").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#5865F2").Padding(5).Text("Kullanıcı Adı").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("E-posta").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Durum").Bold().FontColor(Colors.White);
                        });

                        foreach (var u in users)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(u.UserName ?? "");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(u.Email ?? "");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(u.Status ?? "Offline");
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Sayfa ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", "Disaccord_Kullanici_Raporu.pdf");
            }
        }

        // ----------------------------------------------------
        // CHANNELS REPORT
        // ----------------------------------------------------
        public IActionResult ExportChannelsExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Disaccord Developer");
            var channels = _unitOfWork.Channel.GetAll(includeProperties: "Server").ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Disaccord Kanalları");
                worksheet.Cells[1, 1].Value = "Kanal ID";
                worksheet.Cells[1, 2].Value = "Kanal Adı";
                worksheet.Cells[1, 3].Value = "Kanal Tipi";
                worksheet.Cells[1, 4].Value = "Sunucu Adı";

                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(88, 101, 242));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var c in channels)
                {
                    worksheet.Cells[row, 1].Value = c.Id;
                    worksheet.Cells[row, 2].Value = c.Name;
                    worksheet.Cells[row, 3].Value = c.Type.ToString();
                    worksheet.Cells[row, 4].Value = c.Server?.Name ?? "Bilinmiyor";
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Disaccord_Kanal_Raporu.xlsx");
            }
        }

        public IActionResult ExportChannelsPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var channels = _unitOfWork.Channel.GetAll(includeProperties: "Server").ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial).FontSize(11));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("DISACCORD KANAL RAPORU").FontSize(20).Bold().FontColor("#5865F2");
                            col.Item().Text("Sistemdeki Kayıtlı Kanal Listesi").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#5865F2").Padding(5).Text("ID").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Kanal Adı").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Tipi").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Sunucu").Bold().FontColor(Colors.White);
                        });

                        foreach (var c in channels)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(c.Id.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(c.Name);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(c.Type == ChannelType.Text ? "Metin" : "Ses");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(c.Server?.Name ?? "Bilinmiyor");
                        }
                    });

                    page.Footer().AlignCenter().Text(x => { x.Span("Sayfa "); x.CurrentPageNumber(); x.Span(" / "); x.TotalPages(); });
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", "Disaccord_Kanal_Raporu.pdf");
            }
        }

        // ----------------------------------------------------
        // MESSAGES REPORT
        // ----------------------------------------------------
        public IActionResult ExportMessagesExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Disaccord Developer");
            var messages = _unitOfWork.Message.GetAll(includeProperties: "Sender,Channel,Channel.Server").ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Disaccord Mesajları");
                worksheet.Cells[1, 1].Value = "Mesaj ID";
                worksheet.Cells[1, 2].Value = "Gönderen";
                worksheet.Cells[1, 3].Value = "Sunucu";
                worksheet.Cells[1, 4].Value = "Kanal";
                worksheet.Cells[1, 5].Value = "İçerik";
                worksheet.Cells[1, 6].Value = "Tarih";

                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(88, 101, 242));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var m in messages)
                {
                    worksheet.Cells[row, 1].Value = m.Id;
                    worksheet.Cells[row, 2].Value = m.Sender?.UserName ?? "Bilinmiyor";
                    worksheet.Cells[row, 3].Value = m.Channel?.Server?.Name ?? "Bilinmiyor";
                    worksheet.Cells[row, 4].Value = m.Channel?.Name ?? "Bilinmiyor";
                    worksheet.Cells[row, 5].Value = m.Content;
                    worksheet.Cells[row, 6].Value = m.SendDate.ToLocalTime().ToString("g");
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Disaccord_Mesaj_Raporu.xlsx");
            }
        }

        public IActionResult ExportMessagesPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var messages = _unitOfWork.Message.GetAll(includeProperties: "Sender,Channel,Channel.Server").ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial).FontSize(11));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("DISACCORD MESAJ RAPORU").FontSize(20).Bold().FontColor("#5865F2");
                            col.Item().Text("Sistemdeki Genel Sohbet Geçmişi").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(5);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#5865F2").Padding(5).Text("ID").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Gönderen").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Konum").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Mesaj").Bold().FontColor(Colors.White);
                        });

                        foreach (var m in messages)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(m.Id.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(m.Sender?.UserName ?? "Bilinmiyor");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{m.Channel?.Server?.Name ?? "Dm"} > #{m.Channel?.Name}");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(m.Content ?? "");
                        }
                    });

                    page.Footer().AlignCenter().Text(x => { x.Span("Sayfa "); x.CurrentPageNumber(); x.Span(" / "); x.TotalPages(); });
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", "Disaccord_Mesaj_Raporu.pdf");
            }
        }

        // ----------------------------------------------------
        // SERVER MEMBERS REPORT
        // ----------------------------------------------------
        public IActionResult ExportMembersExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Disaccord Developer");
            var members = _unitOfWork.ServerMember.GetAll(includeProperties: "Server,User").ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sunucu Üyeleri");
                worksheet.Cells[1, 1].Value = "Kullanıcı Adı";
                worksheet.Cells[1, 2].Value = "E-posta";
                worksheet.Cells[1, 3].Value = "Sunucu Adı";
                worksheet.Cells[1, 4].Value = "Rol";
                worksheet.Cells[1, 5].Value = "Katılma Tarihi";

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(88, 101, 242));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var m in members)
                {
                    worksheet.Cells[row, 1].Value = m.User?.UserName ?? "Bilinmiyor";
                    worksheet.Cells[row, 2].Value = m.User?.Email ?? "Bilinmiyor";
                    worksheet.Cells[row, 3].Value = m.Server?.Name ?? "Bilinmiyor";
                    worksheet.Cells[row, 4].Value = m.ServerRole;
                    worksheet.Cells[row, 5].Value = m.JoinedDate.ToLocalTime().ToString("g");
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Disaccord_Uye_Raporu.xlsx");
            }
        }

        public IActionResult ExportMembersPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var members = _unitOfWork.ServerMember.GetAll(includeProperties: "Server,User").ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial).FontSize(11));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("DISACCORD SUNUCU ÜYE RAPORU").FontSize(20).Bold().FontColor("#5865F2");
                            col.Item().Text("Sistemdeki Sunucu Üyelikleri ve Rolleri").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#5865F2").Padding(5).Text("Kullanıcı").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Sunucu").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Rol").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Katılma").Bold().FontColor(Colors.White);
                        });

                        foreach (var m in members)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(m.User?.UserName ?? "Bilinmiyor");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(m.Server?.Name ?? "Bilinmiyor");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(m.ServerRole ?? "Member");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(m.JoinedDate.ToLocalTime().ToString("g"));
                        }
                    });

                    page.Footer().AlignCenter().Text(x => { x.Span("Sayfa "); x.CurrentPageNumber(); x.Span(" / "); x.TotalPages(); });
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", "Disaccord_Uye_Raporu.pdf");
            }
        }

        // ----------------------------------------------------
        // FRIENDSHIPS REPORT
        // ----------------------------------------------------
        public IActionResult ExportFriendshipsExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Disaccord Developer");
            var friendships = _unitOfWork.Friendship.GetAll(includeProperties: "Sender,Receiver").ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Disaccord Arkadaşlıklar");
                worksheet.Cells[1, 1].Value = "İlişki ID";
                worksheet.Cells[1, 2].Value = "Gönderen";
                worksheet.Cells[1, 3].Value = "Alıcı";
                worksheet.Cells[1, 4].Value = "Durum";
                worksheet.Cells[1, 5].Value = "Oluşturulma Tarihi";

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(88, 101, 242));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 2;
                foreach (var f in friendships)
                {
                    worksheet.Cells[row, 1].Value = f.Id;
                    worksheet.Cells[row, 2].Value = f.Sender?.UserName ?? "Bilinmiyor";
                    worksheet.Cells[row, 3].Value = f.Receiver?.UserName ?? "Bilinmiyor";
                    worksheet.Cells[row, 4].Value = f.Status.ToString();
                    worksheet.Cells[row, 5].Value = f.CreatedDate.ToLocalTime().ToString("g");
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Disaccord_Arkadaslik_Raporu.xlsx");
            }
        }

        public IActionResult ExportFriendshipsPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var friendships = _unitOfWork.Friendship.GetAll(includeProperties: "Sender,Receiver").ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial).FontSize(11));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("DISACCORD ARKADAŞLIK RAPORU").FontSize(20).Bold().FontColor("#5865F2");
                            col.Item().Text("Sistemdeki Kullanıcı İlişkileri listesi").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#5865F2").Padding(5).Text("ID").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Gönderen").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Alıcı").Bold().FontColor(Colors.White);
                            header.Cell().Background("#5865F2").Padding(5).Text("Durum").Bold().FontColor(Colors.White);
                        });

                        foreach (var f in friendships)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(f.Id.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(f.Sender?.UserName ?? "Bilinmiyor");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(f.Receiver?.UserName ?? "Bilinmiyor");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(f.Status.ToString());
                        }
                    });

                    page.Footer().AlignCenter().Text(x => { x.Span("Sayfa "); x.CurrentPageNumber(); x.Span(" / "); x.TotalPages(); });
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", "Disaccord_Arkadaslik_Raporu.pdf");
            }
        }
    }
}
