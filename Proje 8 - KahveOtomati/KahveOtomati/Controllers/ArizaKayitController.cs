using KahveOtomati.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KahveOtomati.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArizaKayitController : ControllerBase
    {
        private readonly KahveOtomatiContext _context;

        public ArizaKayitController(KahveOtomatiContext context)
        {
            _context = context;
        }

        // GET: api/ArizaKayit - Tüm arıza kayıtlarını getir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArizaKayit>>> GetArizaKayitlari()
        {
            var arizalar = await _context.ArizaKayitlari.ToListAsync();
            return Ok(arizalar);
        }

        // GET: api/ArizaKayit/5 - ID'ye göre arıza kaydı getir
        [HttpGet("{id}")]
        public async Task<ActionResult<ArizaKayit>> GetArizaKayit(int id)
        {
            var ariza = await _context.ArizaKayitlari.FindAsync(id);

            if (ariza == null)
            {
                return NotFound();
            }

            return Ok(ariza);
        }

        // POST: api/ArizaKayit - Yeni arıza kaydı ekle
        [HttpPost]
        public async Task<ActionResult<ArizaKayit>> PostArizaKayit(ArizaKayit ariza)
        {
            _context.ArizaKayitlari.Add(ariza);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArizaKayit), new { id = ariza.ArizaID }, ariza);
        }

        // PUT: api/ArizaKayit/5 - Arıza kaydı güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArizaKayit(int id, ArizaKayit ariza)
        {
            if (id != ariza.ArizaID)
            {
                return BadRequest();
            }

            _context.Entry(ariza).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ArizaKayitlari.Any(e => e.ArizaID == id))
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

        // DELETE: api/ArizaKayit/5 - Arıza kaydı sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArizaKayit(int id)
        {
            var ariza = await _context.ArizaKayitlari.FindAsync(id);

            if (ariza == null)
            {
                return NotFound();
            }

            _context.ArizaKayitlari.Remove(ariza);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
