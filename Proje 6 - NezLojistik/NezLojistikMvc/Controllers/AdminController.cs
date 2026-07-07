using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NezLojistikMvc.Models;
using NezLojistikMvc.MVC_Data;
using System.Text.Json;
using System.Text;

namespace NezLojistikMvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly MvcDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(MvcDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // ==================== ADMIN LOGIN ====================

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AdminEmail") != null)
            {
                return RedirectToAction("Panel");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == model.Email && a.Password == model.Password);

            if (admin == null)
            {
                ViewBag.Error = "E-posta veya şifre hatalı!";
                return View(model);
            }

            HttpContext.Session.SetString("AdminEmail", admin.Email);
            HttpContext.Session.SetString("AdminFullName", admin.FullName);
            HttpContext.Session.SetInt32("AdminId", admin.Id);

            return RedirectToAction("Panel");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ==================== ADMIN PANEL ====================

        public IActionResult Panel()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.AdminName = HttpContext.Session.GetString("AdminFullName");
            return View();
        }

        // ==================== DEPOTS (API'den HttpClient ile) ====================

        public async Task<IActionResult> Depots()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
                return RedirectToAction("Login");

            ViewBag.AdminName = HttpContext.Session.GetString("AdminFullName");

            var client = _httpClientFactory.CreateClient("NezApi");
            var response = await client.GetAsync("Depots/GetDepots");
            var depots = new List<Depot>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                depots = JsonSerializer.Deserialize<List<Depot>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Depot>();
            }

            return View(depots);
        }

        [HttpPost]
        public async Task<IActionResult> AddDepot(Depot depot)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("NezApi");
            var json = JsonSerializer.Serialize(depot);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("Depots/AddDepot", content);

            return RedirectToAction("Depots");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDepot(int id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("NezApi");
            await client.DeleteAsync($"Depots/DeleteDepot/{id}");

            return RedirectToAction("Depots");
        }

        // ==================== DRIVERS (API'den HttpClient ile) ====================

        public async Task<IActionResult> Drivers()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
                return RedirectToAction("Login");

            ViewBag.AdminName = HttpContext.Session.GetString("AdminFullName");

            var client = _httpClientFactory.CreateClient("NezApi");
            var response = await client.GetAsync("Drivers/GetDrivers");
            var drivers = new List<Driver>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                drivers = JsonSerializer.Deserialize<List<Driver>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Driver>();
            }

            return View(drivers);
        }

        [HttpPost]
        public async Task<IActionResult> AddDriver(Driver driver)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("NezApi");
            var json = JsonSerializer.Serialize(driver);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("Drivers/AddDriver", content);

            return RedirectToAction("Drivers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("NezApi");
            await client.DeleteAsync($"Drivers/DeleteDriver/{id}");

            return RedirectToAction("Drivers");
        }

        // ==================== VEHICLES (API'den HttpClient ile) ====================

        public async Task<IActionResult> Vehicles()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
                return RedirectToAction("Login");

            ViewBag.AdminName = HttpContext.Session.GetString("AdminFullName");

            var client = _httpClientFactory.CreateClient("NezApi");
            var response = await client.GetAsync("Vehicles/GetVehicles");
            var vehicles = new List<Vehicle>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                vehicles = JsonSerializer.Deserialize<List<Vehicle>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Vehicle>();
            }

            return View(vehicles);
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle(Vehicle vehicle)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("NezApi");
            var json = JsonSerializer.Serialize(vehicle);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("Vehicles/AddVehicle", content);

            return RedirectToAction("Vehicles");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("NezApi");
            await client.DeleteAsync($"Vehicles/DeleteVehicle/{id}");

            return RedirectToAction("Vehicles");
        }
    }
}
