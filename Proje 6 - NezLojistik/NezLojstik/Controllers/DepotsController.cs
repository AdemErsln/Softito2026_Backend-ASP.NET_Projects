using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NezLojstikApi.Models;

namespace NezLojstikApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepotsController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public DepotsController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetDepots")]
        public async Task<IEnumerable<Depot>> GetDepots()
        {
            return await _context.Depots.ToListAsync();
        }

        [HttpGet]
        [Route("GetDepotById/{id}")]
        public async Task<Depot?> GetDepotById(int id)
        {
            return await _context.Depots.FindAsync(id);
        }

        [HttpPost]
        [Route("AddDepot")]
        public async Task<Depot> AddDepot(Depot depot)
        {
            _context.Depots.Add(depot);
            await _context.SaveChangesAsync();

            return depot;
        }

        [HttpPut]
        [Route("UpdateDepot/{id}")]
        public async Task<Depot> UpdateDepot(Depot depot)
        {
            _context.Depots.Update(depot);
            await _context.SaveChangesAsync();

            return depot;
        }

        [HttpDelete]
        [Route("DeleteDepot/{id}")]
        public bool DeleteDepot(int id)
        {
            bool process = false;

            var result = _context.Depots.Find(id);

            if (result != null)
            {
                process = true;

                _context.Depots.Remove(result);
                _context.SaveChanges();
            }

            return process;
        }
    }
}