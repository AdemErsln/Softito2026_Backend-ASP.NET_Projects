using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NezLojstikApi.Models;

namespace NezLojstikApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public VehiclesController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetVehicles")]
        public async Task<IEnumerable<Vehicle>> GetVehicles()
        {
            return await _context.Vehicles.ToListAsync();
        }

        [HttpGet]
        [Route("GetVehicleById/{id}")]
        public async Task<Vehicle> GetVehicleById(int id)
        {
            return await _context.FindAsync<Vehicle>(id);
        }

        [HttpPost]
        [Route("AddVehicle")]
        public async Task<Vehicle> AddVehicle(Vehicle vehicle)
        {
            _context.Add(vehicle);
            await _context.SaveChangesAsync();

            return vehicle;
        }

        [HttpPut]
        [Route("UpdateVehicle/{id}")]
        public async Task<Vehicle> UpdateVehicle(Vehicle vehicle)
        {
            _context.Update(vehicle);
            await _context.SaveChangesAsync();

            return vehicle;
        }

        [HttpDelete]
        [Route("DeleteVehicle/{id}")]
        public bool DeleteVehicle(int id)
        {
            bool process = false;

            var result = _context.Vehicles.Find(id);

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