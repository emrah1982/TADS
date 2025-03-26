using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TADS.API.Data;
using TADS.API.Models;

namespace TADS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FertilizerTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FertilizerTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FertilizerType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FertilizerType>>> GetFertilizerTypes()
        {
            return await _context.FertilizerTypes.OrderBy(ft => ft.Name).ToListAsync();
        }

        // GET: api/FertilizerType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FertilizerType>> GetFertilizerType(int id)
        {
            var fertilizerType = await _context.FertilizerTypes.FindAsync(id);

            if (fertilizerType == null)
            {
                return NotFound(new { success = false, message = "Gübre türü bulunamadı" });
            }

            return fertilizerType;
        }

        // POST: api/FertilizerType
        [HttpPost]
        public async Task<ActionResult<FertilizerType>> CreateFertilizerType(FertilizerType fertilizerType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Geçersiz gübre türü verisi", errors = ModelState });
            }

            // Aynı isimde gübre türü var mı kontrol et
            if (await _context.FertilizerTypes.AnyAsync(ft => ft.Name.ToLower() == fertilizerType.Name.ToLower()))
            {
                return BadRequest(new { success = false, message = "Bu isimde bir gübre türü zaten mevcut" });
            }

            _context.FertilizerTypes.Add(fertilizerType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFertilizerType), new { id = fertilizerType.Id }, fertilizerType);
        }

        // PUT: api/FertilizerType/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFertilizerType(int id, FertilizerType fertilizerType)
        {
            if (id != fertilizerType.Id)
            {
                return BadRequest(new { success = false, message = "ID uyuşmazlığı" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Geçersiz gübre türü verisi", errors = ModelState });
            }

            // Aynı isimde başka bir gübre türü var mı kontrol et
            if (await _context.FertilizerTypes.AnyAsync(ft => ft.Name.ToLower() == fertilizerType.Name.ToLower() && ft.Id != id))
            {
                return BadRequest(new { success = false, message = "Bu isimde bir gübre türü zaten mevcut" });
            }

            _context.Entry(fertilizerType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FertilizerTypeExists(id))
                {
                    return NotFound(new { success = false, message = "Gübre türü bulunamadı" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/FertilizerType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFertilizerType(int id)
        {
            var fertilizerType = await _context.FertilizerTypes.FindAsync(id);
            if (fertilizerType == null)
            {
                return NotFound(new { success = false, message = "Gübre türü bulunamadı" });
            }

            // Bu gübre türü herhangi bir gübreleme kaydında kullanılıyor mu kontrol et
            if (await _context.Fertilizations.AnyAsync(f => f.FertilizerTypeId == id))
            {
                return BadRequest(new { success = false, message = "Bu gübre türü gübreleme kayıtlarında kullanıldığı için silinemez" });
            }

            _context.FertilizerTypes.Remove(fertilizerType);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Gübre türü başarıyla silindi" });
        }

        private bool FertilizerTypeExists(int id)
        {
            return _context.FertilizerTypes.Any(e => e.Id == id);
        }
    }
}
