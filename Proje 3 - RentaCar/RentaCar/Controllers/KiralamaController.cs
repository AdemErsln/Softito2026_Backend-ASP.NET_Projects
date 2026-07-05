using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentacCar.Model;
using System.Linq;
using RentaCar.Data;

namespace RentACar.Controllers
{
    public class KiralamaController : Controller
    {
        private readonly ApplicationDBContext _dBContext;
        public KiralamaController(ApplicationDBContext dBContext) { _dBContext = dBContext; }

        public IActionResult Index()
        {
            // İlişkili tabloları Include ile bağlayarak listeliyoruz
            var kiralamalar = _dBContext.Kiralamas
                .Include(k => k.Kullanicilar)
                .Include(k => k.Araclar)
                .ToList();
            return View(kiralamalar);
        }

        // Dropdown listelerini dolduran yardımcı metot
        private void DropdownListeleriDoldur()
        {
            ViewBag.Musteriler = new SelectList(_dBContext.kullanicilars
                .Select(m => new { m.Id, Isim = m.AdSoyad }), "Id", "Isim");

            ViewBag.Araclar = new SelectList(_dBContext.Araclar
                .Select(a => new { a.CarId, Bilgi = a.Plaka + " - " + a.Marka + " " + a.Model }), "CarId", "Bilgi");
        }

        // GET: /Kiralama/Create
        [HttpGet]
        public IActionResult Create()
        {
            DropdownListeleriDoldur();
            return View(new Kiralama());
        }

        // POST: /Kiralama/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Kiralama kiralama)
        {
            if (ModelState.IsValid)
            {
                _dBContext.Kiralamas.Add(kiralama);

                // Sözleşme açıldığında aracın durumunu otomatik "Kirada" yapalım
                var arac = _dBContext.Araclar.Find(kiralama.CarId);
                if (arac != null) arac.Durum = "Kirada";

                _dBContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            DropdownListeleriDoldur();
            return View(kiralama);
        }

        // GET: /Kiralama/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var kiralama = _dBContext.Kiralamas.Find(id);
            if (kiralama == null) return NotFound();

            DropdownListeleriDoldur();
            return View(kiralama);
        }

        // POST: /Kiralama/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Kiralama kiralama)
        {
            if (id != kiralama.RentalId) return NotFound();

            if (ModelState.IsValid)
            {
                _dBContext.Kiralamas.Update(kiralama);

                // Eğer sözleşme durumu kapatıldıysa aracı tekrar "Müsait" durumuna çekelim
                if (kiralama.Durum == "Kapatıldı")
                {
                    var arac = _dBContext.Araclar.Find(kiralama.CarId);
                    if (arac != null) arac.Durum = "Müsait";
                }

                _dBContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            DropdownListeleriDoldur();
            return View(kiralama);
        }
    }
}