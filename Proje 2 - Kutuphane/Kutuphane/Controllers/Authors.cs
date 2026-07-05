using Kutuphane.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kutuphane.Controllers
{
    public class Authors : Controller
    {

        private readonly AppDbContext _context;
        public Authors(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var veri = _context.Authors.ToList();
            return View(veri);
        }

        public IActionResult Create()
        {
            return View();  
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken] // Güvenlik için CSRF saldırılarını engeller
        public IActionResult Create(Author author) // 'Authors' yerine 'Author' olarak güncellendi
        {
            // Form verileri kurallara uymuyorsa sayfayı hatalarla birlikte geri yükle
            if (!ModelState.IsValid)
            {
                return View(author);
            }

            // Veritabanına ekleme işlemi
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Index sayfasına mor temalı bir başarı bildirimi fırlatmak için TempData kullanıyoruz
            TempData["SuccessMessage"] = $"{author.Name} isimli yazar başarıyla sisteme eklendi.";

            return RedirectToAction(nameof(Index)); // String yazmak yerine nameof kullanmak daha güvenlidir
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Silinecek yazarı bul (Model adın muhtemelen Author)
            var author = _context.Authors.Find(id);

            if (author == null)
            {
                return NotFound();
            }

            return View(author); // Onay sayfasına yazarı gönderiyoruz
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id) // Id üzerinden silmek en temiz yoldur
        {
            var author = _context.Authors.Find(id);

            if (author != null)
            {
                _context.Authors.Remove(author);
                _context.SaveChanges();

                // Silme başarılı mesajı fırlatalım
                TempData["SuccessMessage"] = "Yazar başarıyla silindi.";
            }

            // İşlem bitince ana listeye (Index) yönlendiriyoruz
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Güncellenecek yazarı veritabanından buluyoruz
            var author = _context.Authors.Find(id);

            if (author == null)
            {
                return NotFound(); // Yazar bulunamazsa 404 sayfasına atar
            }

            return View(author); // Yazar bilgilerini Formun içine dolması için View'a gönderiyoruz
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Author author)
        {
            
            if (id != author.AuthorId)
            {
                return BadRequest();
            }

            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author); // Değişiklikleri güncelle
                    _context.SaveChanges();  // Veritabanına kaydet

                    TempData["SuccessMessage"] = $"{author.Name} isimli yazar başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu.");
                }
            }

            // Eğer bir hata varsa formu kapatma, hatalarla birlikte aynı sayfayı tekrar göster
            return View(author);
        }
    }
}
