using Microsoft.AspNetCore.Mvc;

using RentaCar.Data;
namespace RentaCar.Controllers
{
    public class AdminController : Controller
    {

        ApplicationDBContext _dBContext;

        public AdminController(ApplicationDBContext dbContext)
        {
            this._dBContext = dbContext;
        }
        public IActionResult Index()

        {
            var result = _dBContext.Araclar.ToList();
            return View(result);
        }
    }
}
