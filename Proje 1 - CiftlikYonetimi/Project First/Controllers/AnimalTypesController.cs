using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_First.Data;
using Project_First.Models;

namespace Project_First.Controllers
{
    public class AnimalTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AnimalTypesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // 1. LISTELEME (Index)
        public IActionResult Index()
        {
            // ToListAsync() yerine direkt senkron olan ToList() kullandık
            var types = _context.AnimalType.ToList();
            return View(types);
        }

        // 2. EKLEME (GET)
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        // 3. EKLEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create( AnimalTypes animalType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(animalType);
                _context.SaveChanges(); // Senkron kaydetme metodu
                return RedirectToAction(nameof(Index));
            }
            return View(animalType);
        }

        // 4. DÜZENLEME (GET)
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

  
            var animalType = _context.AnimalType.Find(id);
            if (animalType == null) return NotFound();

            return View(animalType);
        }

        // 5. DÜZENLEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,animal_type")] AnimalTypes animalType)
        {
            if (id != animalType.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(animalType);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(animalType);
        }

        // 6. SİLME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var animalType = _context.AnimalType.Find(id);
            if (animalType != null)
            {
                _context.AnimalType.Remove(animalType);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}