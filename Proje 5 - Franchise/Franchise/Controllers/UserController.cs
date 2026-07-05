using Franchise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace Franchise.Controllers
{
    public class UserController : Controller
    {
        ApplicationDbContext _dbcontex;

        public UserController(ApplicationDbContext dbcontex)
        {
            _dbcontex = dbcontex;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCurrentPackage()
        {
            try
            {
                // 1. 4 ID'li kullanıcıyı ve onun paket ilişkisini (Include ile) veritabanından çekiyoruz
                var currentBuyer = _dbcontex.FranchiseBuyers
                                             .Include(b => b.FranchisePackage) // İlişkili paket tablosunu dahil et
                                             .FirstOrDefault(b => b.BuyerID == 2);

                // 2. Kullanıcı var mı kontrolü
                if (currentBuyer == null)
                {
                    return Json(new { success = false, message = "2 ID'li kullanıcı bulunamadı." });
                }

                // 3. Kullanıcının paketi var mı kontrolü (PackageID null olabilir diye)
                if (currentBuyer.FranchisePackage == null)
                {
                    return Json(new { success = false, message = $"{currentBuyer.BuyerName} isimli kullanıcının tanımlı bir paketi yok." });
                }

                // 4. Paket bulunduysa, paket bilgilerini JSON olarak dönüyoruz
                return Json(new
                {
                    success = true,
                    buyerName = currentBuyer.BuyerName,
                    packageName = currentBuyer.FranchisePackage.PackageName, // Paketin adı
                    packageId = currentBuyer.FranchisePackage.PackageID
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpGet]

        public JsonResult GetAvailableUpgrades()
        {
            var result = _dbcontex.FranchisePackages.ToList();
            return Json(result);
        }

        [HttpPost]

        public JsonResult UpgradePackage(int packageid)
        {

            try
            {
                var targetPackage = _dbcontex.FranchisePackages
                                             .FirstOrDefault(p => p.PackageID == packageid);

                if (targetPackage == null)
                {
                    return Json(new { success = false, message = $"Seçilen paket bulunamadı , Seçilen paket id {packageid}" });
                }

                // 1. Güncellenecek veriyi çekiyoruz
                var currentBuyer = _dbcontex.FranchiseBuyers.FirstOrDefault(b => b.BuyerID == 2);

                if (currentBuyer != null)
                {
                    // 2. Değerleri atıyoruz (Örn: Paket ID'sini veya adını güncelliyoruz)
                    currentBuyer.PackageID = targetPackage.PackageID;
                    _dbcontex.SaveChanges();
                    // currentBuyer.PackageName = targetPackage.PackageName; // Kendi property isimlerine göre uyarla

                    // 3. Değişiklikleri kaydediyoruz (EF Core değişen alanları anlar ve günceller)
                    _dbcontex.SaveChanges();
                }
                else
                {
                    return Json(new { success = false, message = "Alıcı bulunamadı." });
                }

                return Json(new { success = true, message = $"Paketiniz başarıyla {targetPackage.PackageName} modeline yükseltildi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }

        }


    }
}
