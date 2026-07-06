using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_First.Data;
using Project_First.Models;
using System.Runtime.InteropServices.Marshalling;

namespace Project_First.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var animals = _context.Animals
    .Include(x => x.AnimalType)
    .ToList();

            return View(animals);
            
        }


        public IActionResult AddAnimal()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult AddAnimal(Animals animal)
        {
            if (!ModelState.IsValid)
                return View(animal);

            _context.Animals.Add(animal);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
