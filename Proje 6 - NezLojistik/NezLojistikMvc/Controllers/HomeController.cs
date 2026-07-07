using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using NezLojistikMvc.Models;

namespace NezLojistikMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Account/Login")]
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [Route("Admin")]
        public async Task<IActionResult> Admin()
        {
            var model = new AdminViewModel();
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(3);
            
            // Try fetching depots
            try
            {
                var response = await client.GetAsync("http://localhost:5046/api/Depots/GetDepots");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    model.Depots = JsonSerializer.Deserialize<List<DepotViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not fetch depots: {ex.Message}");
            }

            // Try fetching drivers
            try
            {
                var response = await client.GetAsync("http://localhost:5046/api/Drivers/GetDrivers");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    model.Drivers = JsonSerializer.Deserialize<List<DriverViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not fetch drivers: {ex.Message}");
            }

            // Try fetching vehicles
            try
            {
                var response = await client.GetAsync("http://localhost:5046/api/Vehicles/GetVehicles");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    model.Vehicles = JsonSerializer.Deserialize<List<VehicleViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not fetch vehicles: {ex.Message}");
            }

            // Fallback to mock data if empty
            if (model.Depots.Count == 0)
            {
                model.Depots = new List<DepotViewModel>
                {
                    new() { Id = 1, DepotName = "Merkez Depo", Address = "Atatürk Cad. No:1", City = "İstanbul" },
                    new() { Id = 2, DepotName = "Ege Bölge Depo", Address = "Liman Sok. No:45", City = "İzmir" },
                    new() { Id = 3, DepotName = "İç Anadolu Depo", Address = "Sanayi Sitesi 4. Blok", City = "Ankara" }
                };
            }

            if (model.Drivers.Count == 0)
            {
                model.Drivers = new List<DriverViewModel>
                {
                    new() { Id = 1, FullName = "Hasan Kaya", PhoneNumber = "0532 111 2233", LicenseNumber = "A-B-C-D-E" },
                    new() { Id = 2, FullName = "Kemal Yıldız", PhoneNumber = "0544 222 3344", LicenseNumber = "A-B-C" },
                    new() { Id = 3, FullName = "Mehmet Demir", PhoneNumber = "0555 333 4455", LicenseNumber = "B-C-E" }
                };
            }

            if (model.Vehicles.Count == 0)
            {
                model.Vehicles = new List<VehicleViewModel>
                {
                    new() { Id = 1, Plate = "34TC1010", Brand = "Mercedes Actros", Capacity = 24000 },
                    new() { Id = 2, Plate = "35KS999", Brand = "Volvo FH", Capacity = 26000 },
                    new() { Id = 3, Plate = "06ANK06", Brand = "Scania R500", Capacity = 25000 }
                };
            }

            return View("~/Views/Admin/Index.cshtml", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class AdminViewModel
    {
        public List<DepotViewModel> Depots { get; set; } = new();
        public List<DriverViewModel> Drivers { get; set; } = new();
        public List<VehicleViewModel> Vehicles { get; set; } = new();
    }

    public class DepotViewModel
    {
        public int Id { get; set; }
        public string DepotName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
    }

    public class DriverViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string LicenseNumber { get; set; } = "";
    }

    public class VehicleViewModel
    {
        public int Id { get; set; }
        public string Plate { get; set; } = "";
        public string Brand { get; set; } = "";
        public int Capacity { get; set; }
    }
}
