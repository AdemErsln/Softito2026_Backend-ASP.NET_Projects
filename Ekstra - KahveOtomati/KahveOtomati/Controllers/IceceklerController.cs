using KahveOtomati.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KahveOtomati.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IceceklerController : ControllerBase
    {
        private readonly KahveOtomatiContext _context;

        public IceceklerController(KahveOtomatiContext context)
        {
            _context = context;
        }

        // GET: api/Icecekler - Tüm içecekleri getir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Icecek>>> GetIcecekler()
        {
            var icecekler = await _context.Icecekler.ToListAsync();
            return Ok(icecekler);
        }

        // GET: api/Icecekler/5 - ID'ye göre içecek getir
        [HttpGet("{id}")]
        public async Task<ActionResult<Icecek>> GetIcecek(int id)
        {
            var icecek = await _context.Icecekler.FindAsync(id);

            if (icecek == null)
            {
                return NotFound();
            }

            return Ok(icecek);
        }

        // POST: api/Icecekler - Yeni içecek ekle
        [HttpPost]
        public async Task<ActionResult<Icecek>> PostIcecek(Icecek icecek)
        {
            _context.Icecekler.Add(icecek);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIcecek), new { id = icecek.IcecekID }, icecek);
        }

        // PUT: api/Icecekler/5 - İçecek güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIcecek(int id, Icecek icecek)
        {
            if (id != icecek.IcecekID)
            {
                return BadRequest();
            }

            _context.Entry(icecek).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Icecekler.Any(e => e.IcecekID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Icecekler/5 - İçecek sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIcecek(int id)
        {
            var icecek = await _context.Icecekler.FindAsync(id);

            if (icecek == null)
            {
                return NotFound();
            }

            _context.Icecekler.Remove(icecek);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
