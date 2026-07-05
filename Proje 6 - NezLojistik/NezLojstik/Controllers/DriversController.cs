using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NezLojstikApi.Models;

namespace NezLojstikApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public DriversController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetDrivers")]
        public async Task<IEnumerable<Driver>> GetDrivers()
        {
            return await _context.Drivers.ToListAsync();
        }

        [HttpGet]
        [Route("GetDriverById/{id}")]
        public async Task<Driver> GetDriverById(int id)
        {
            return await _context.FindAsync<Driver>(id);
        }

        [HttpPost]
        [Route("AddDriver")]
        public async Task<Driver> AddDriver(Driver driver)
        {
            _context.Add(driver);
            await _context.SaveChangesAsync();

            return driver;
        }

        [HttpPut]
        [Route("UpdateDriver/{id}")]
        public async Task<Driver> UpdateDriver(Driver driver)
        {
            _context.Update(driver);
            await _context.SaveChangesAsync();

            return driver;
        }

        [HttpDelete]
        [Route("DeleteDriver/{id}")]
        public bool DeleteDriver(int id)
        {
            bool process = false;

            var result = _context.Drivers.Find(id);

            if (result != null)
            {
                process = true;
                _context.Remove(result);
                _context.SaveChanges();
            }

            return process;
        }
    }
}