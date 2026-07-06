using KahveOtomati.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KahveOtomati.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MalzemeStokController : ControllerBase
    {
        private readonly KahveOtomatiContext _context;

        public MalzemeStokController(KahveOtomatiContext context)
        {
            _context = context;
        }

        // GET: api/MalzemeStok - Tüm stokları getir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MalzemeStok>>> GetMalzemeStoklari()
        {
            var stoklar = await _context.MalzemeStoklari.ToListAsync();
            return Ok(stoklar);
        }

        // GET: api/MalzemeStok/5 - ID'ye göre stok getir
        [HttpGet("{id}")]
        public async Task<ActionResult<MalzemeStok>> GetMalzemeStok(int id)
        {
            var stok = await _context.MalzemeStoklari.FindAsync(id);

            if (stok == null)
            {
                return NotFound();
            }

            return Ok(stok);
        }

        // POST: api/MalzemeStok - Yeni stok ekle
        [HttpPost]
        public async Task<ActionResult<MalzemeStok>> PostMalzemeStok(MalzemeStok stok)
        {
            _context.MalzemeStoklari.Add(stok);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMalzemeStok), new { id = stok.StokID }, stok);
        }

        // PUT: api/MalzemeStok/5 - Stok güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMalzemeStok(int id, MalzemeStok stok)
        {
            if (id != stok.StokID)
            {
                return BadRequest();
            }

            _context.Entry(stok).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.MalzemeStoklari.Any(e => e.StokID == id))
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

        // DELETE: api/MalzemeStok/5 - Stok sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMalzemeStok(int id)
        {
            var stok = await _context.MalzemeStoklari.FindAsync(id);

            if (stok == null)
            {
                return NotFound();
            }

            _context.MalzemeStoklari.Remove(stok);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
