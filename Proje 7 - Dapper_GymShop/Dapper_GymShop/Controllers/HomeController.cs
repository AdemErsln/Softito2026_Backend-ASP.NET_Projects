using Dapper_GymShop.Models;
using Dapper_GymShop.Models.ShippingCompany;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Dapper;

namespace Dapper_GymShop.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            using (var connection = new SqlConnection(Context.connectionstring))
            {
                var products = connection.Query<Product>("sp_GetIndexProducts", commandType: CommandType.StoredProcedure).ToList();
                var shippings = connection.Query<ShippingCompany>("sp_GetShippingCompanies", commandType: CommandType.StoredProcedure).ToList();

                ViewBag.Shippings = shippings;
                return View(products);
            }
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

        public IActionResult Login()
        {

            return View();
        } 
    }
}
