using Microsoft.AspNetCore.Mvc;
using RentaCar.Data;
using RentacCar.Model;
using System.Linq;

namespace RentACar.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly ApplicationDBContext _dBContext; // Kendi DbContext adınla değiştir
        public KullaniciController(ApplicationDBContext dBContext) { _dBContext = dBContext; }

        public IActionResult Index()
        {
            return View(_dBContext.kullanicilars.ToList());
        }

        // GET: /Kullanici/Create
        [HttpGet]
        public IActionResult Create() => View(new Kullanicilar());

        // POST: /Kullanici/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Kullanicilar musteri)
        {
            if (ModelState.IsValid)
            {
                _dBContext.kullanicilars.Add(musteri);
                _dBContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(musteri);
        }

        // GET: /Kullanici/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var musteri = _dBContext.kullanicilars.Find(id);
            if (musteri == null) return NotFound();
            return View(musteri);
        }

        // POST: /Kullanici/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Kullanicilar musteri)
        {
            if (id != musteri.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _dBContext.kullanicilars.Update(musteri);
                _dBContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(musteri);
        }

        // GET: /Kullanici/Delete/5 (Direkt GET ile Silme)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var musteri = _dBContext.kullanicilars.Find(id);
            if (musteri != null)
            {
                _dBContext.kullanicilars.Remove(musteri);
                _dBContext.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}