using Microsoft.AspNetCore.Mvc;
using NezLojistikMvc.Models;
using System.Diagnostics;

namespace NezLojistikMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Eğer kullanıcı giriş yapmışsa Dashboard'a yönlendir
            if (HttpContext.Session.GetString("UserEmail") != null)
            {
                return RedirectToAction("Dashboard", "Account");
            }
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
