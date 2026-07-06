using KahveOtomati.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KahveOtomati.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SatislarController : ControllerBase
    {
        private readonly KahveOtomatiContext _context;

        public SatislarController(KahveOtomatiContext context)
        {
            _context = context;
        }

        // GET: api/Satislar - Tüm satışları getir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Satis>>> GetSatislar()
        {
            var satislar = await _context.Satislar.ToListAsync();
            return Ok(satislar);
        }

        // GET: api/Satislar/5 - ID'ye göre satış getir
        [HttpGet("{id}")]
        public async Task<ActionResult<Satis>> GetSatis(int id)
        {
            var satis = await _context.Satislar.FindAsync(id);

            if (satis == null)
            {
                return NotFound();
            }

            return Ok(satis);
        }

        // POST: api/Satislar - Yeni satış ekle
        [HttpPost]
        public async Task<ActionResult<Satis>> PostSatis(Satis satis)
        {
            _context.Satislar.Add(satis);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSatis), new { id = satis.SatisID }, satis);
        }

        // PUT: api/Satislar/5 - Satış güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSatis(int id, Satis satis)
        {
            if (id != satis.SatisID)
            {
                return BadRequest();
            }

            _context.Entry(satis).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Satislar.Any(e => e.SatisID == id))
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

        // DELETE: api/Satislar/5 - Satış sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSatis(int id)
        {
            var satis = await _context.Satislar.FindAsync(id);

            if (satis == null)
            {
                return NotFound();
            }

            _context.Satislar.Remove(satis);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
