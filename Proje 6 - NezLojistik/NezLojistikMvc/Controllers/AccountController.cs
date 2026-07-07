using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NezLojistikMvc.Models;

namespace NezLojistikMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly MvcDbContext _context;

        public AccountController(MvcDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                ViewBag.Error = "E-posta veya şifre hatalı!";
                return View(model);
            }

            // Session'a kullanıcı bilgilerini kaydet
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserFullName", user.FullName);
            HttpContext.Session.SetString("UserBolge", user.Bolge);
            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Dashboard");
        }

        // GET: /Account/Dashboard
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.FullName = HttpContext.Session.GetString("UserFullName");
            ViewBag.Bolge = HttpContext.Session.GetString("UserBolge");
            ViewBag.Email = HttpContext.Session.GetString("UserEmail");

            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // E-posta adresi kontrolü
            var exists = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (exists)
            {
                ViewBag.Error = "Bu e-posta adresi sistemde zaten kayıtlı!";
                return View(model);
            }

            var newUser = new NezLojistikMvc.MVC_Data.Users
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                Bolge = model.Bolge
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Kayıt sonrası otomatik login
            HttpContext.Session.SetString("UserEmail", newUser.Email);
            HttpContext.Session.SetString("UserFullName", newUser.FullName);
            HttpContext.Session.SetString("UserBolge", newUser.Bolge);
            HttpContext.Session.SetInt32("UserId", newUser.Id);

            return RedirectToAction("Dashboard");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
