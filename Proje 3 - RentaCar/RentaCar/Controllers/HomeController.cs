using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentaCar.Data;
using RentaCar.Models;
using System.Diagnostics;

namespace RentaCar.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDBContext _dBContext;

        public HomeController(ApplicationDBContext dbContext)
        {
            this._dBContext = dbContext;
        }

        public IActionResult Index()
        {
            ViewBag.ToplamArac = _dBContext.Araclar.Count();

            // 2. Rapor: Şu an Kirada Olan (Müsait Olmayan) Araç Sayısı
            ViewBag.KiradakiArac = _dBContext.Araclar.Count(x => x.Durum == "Kirada");

            // 3. Rapor: Toplam Kayıtlı Müşteri Sayısı
            ViewBag.ToplamMusteri = _dBContext.kullanicilars.Count();

            // 4. Rapor: Kapatılan veya Aktif Tüm Sözleşmelerden Kazanılan Toplam Ciro
            ViewBag.ToplamCiro = _dBContext.Kiralamas.Sum(x => x.ToplamTutar);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
