using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
    public class YieldController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public YieldController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Yield
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Yield>>> GetYields()
        {
            return await _context.Yields
                .Include(y => y.Field)
                .OrderByDescending(y => y.Season)
                .ThenBy(y => y.Field.Name)
                .ToListAsync();
        }

        // GET: api/Yield/field/{fieldId}
        [HttpGet("field/{fieldId}")]
        public async Task<ActionResult<IEnumerable<Yield>>> GetYieldsByField(int fieldId)
        {
            var field = await _context.Fields.FindAsync(fieldId);
            if (field == null)
            {
                return NotFound(new { success = false, message = "Alan bulunamadı" });
            }

            return await _context.Yields
                .Where(y => y.FieldId == fieldId)
                .OrderByDescending(y => y.Season)
                .ToListAsync();
        }

        // GET: api/Yield/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Yield>> GetYield(int id)
        {
            var yield = await _context.Yields
                .Include(y => y.Field)
                .FirstOrDefaultAsync(y => y.Id == id);

            if (yield == null)
            {
                return NotFound(new { success = false, message = "Verim kaydı bulunamadı" });
            }

            return yield;
        }

        // POST: api/Yield
        [HttpPost]
        public async Task<ActionResult<Yield>> CreateYield(Yield yield)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Geçersiz verim verisi", errors = ModelState });
            }

            var field = await _context.Fields.FindAsync(yield.FieldId);
            if (field == null)
            {
                return BadRequest(new { success = false, message = "Geçersiz alan ID" });
            }

            // Aynı alan ve sezon için verim kaydı var mı kontrol et
            if (await _context.Yields.AnyAsync(y => y.FieldId == yield.FieldId && y.Season == yield.Season))
            {
                return BadRequest(new { success = false, message = "Bu alan ve sezon için zaten bir verim kaydı mevcut" });
            }

            yield.CreatedAt = DateTime.Now;
            yield.CreatedBy = User.Identity.Name;

            _context.Yields.Add(yield);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetYield), new { id = yield.Id }, yield);
        }

        // PUT: api/Yield/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateYield(int id, Yield yield)
        {
            if (id != yield.Id)
            {
                return BadRequest(new { success = false, message = "ID uyuşmazlığı" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Geçersiz verim verisi", errors = ModelState });
            }

            // Aynı alan ve sezon için başka bir verim kaydı var mı kontrol et
            if (await _context.Yields.AnyAsync(y => y.FieldId == yield.FieldId && y.Season == yield.Season && y.Id != id))
            {
                return BadRequest(new { success = false, message = "Bu alan ve sezon için zaten bir verim kaydı mevcut" });
            }

            var existingYield = await _context.Yields.FindAsync(id);
            if (existingYield == null)
            {
                return NotFound(new { success = false, message = "Verim kaydı bulunamadı" });
            }

            existingYield.Amount = yield.Amount;
            existingYield.Unit = yield.Unit;
            existingYield.Notes = yield.Notes;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!YieldExists(id))
                {
                    return NotFound(new { success = false, message = "Verim kaydı bulunamadı" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Yield/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteYield(int id)
        {
            var yield = await _context.Yields.FindAsync(id);
            if (yield == null)
            {
                return NotFound(new { success = false, message = "Verim kaydı bulunamadı" });
            }

            _context.Yields.Remove(yield);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Verim kaydı başarıyla silindi" });
        }

        // GET: api/Yield/analytics/field/{fieldId}
        [HttpGet("analytics/field/{fieldId}")]
        public async Task<IActionResult> GetYieldAnalytics(int fieldId)
        {
            var field = await _context.Fields.FindAsync(fieldId);
            if (field == null)
            {
                return NotFound(new { success = false, message = "Alan bulunamadı" });
            }

            var yields = await _context.Yields
                .Where(y => y.FieldId == fieldId)
                .OrderBy(y => y.Season)
                .ToListAsync();

            if (!yields.Any())
            {
                return Ok(new { success = true, message = "Bu alan için verim kaydı bulunamadı", data = new { } });
            }

            // Verim değişim oranını hesapla
            var yieldTrend = new List<object>();
            decimal? previousYield = null;

            foreach (var y in yields)
            {
                decimal? changeRate = null;
                if (previousYield.HasValue && previousYield.Value > 0)
                {
                    changeRate = ((y.Amount - previousYield.Value) / previousYield.Value) * 100;
                }

                yieldTrend.Add(new
                {
                    season = y.Season,
                    amount = y.Amount,
                    unit = y.Unit,
                    changeRate
                });

                previousYield = y.Amount;
            }

            // Ortalama verim
            var averageYield = yields.Average(y => y.Amount);

            // Son sezon verim değişimi
            decimal? lastSeasonChange = null;
            if (yields.Count >= 2)
            {
                var lastYield = yields.OrderByDescending(y => y.Season).First();
                var previousSeasonYield = yields.OrderByDescending(y => y.Season).Skip(1).First();
                lastSeasonChange = ((lastYield.Amount - previousSeasonYield.Amount) / previousSeasonYield.Amount) * 100;
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    yieldTrend,
                    averageYield,
                    lastSeasonChange,
                    unit = yields.First().Unit
                }
            });
        }

        // GET: api/Yield/analytics/compare?fieldIds=1,2,3
        [HttpGet("analytics/compare")]
        public async Task<IActionResult> CompareFieldYields([FromQuery] string fieldIds)
        {
            if (string.IsNullOrEmpty(fieldIds))
            {
                return BadRequest(new { success = false, message = "Alan ID'leri belirtilmedi" });
            }

            var fieldIdList = fieldIds.Split(',').Select(id => int.Parse(id)).ToList();
            
            var fields = await _context.Fields
                .Where(f => fieldIdList.Contains(f.Id))
                .ToListAsync();

            if (fields.Count != fieldIdList.Count)
            {
                return BadRequest(new { success = false, message = "Bir veya daha fazla alan bulunamadı" });
            }

            var yields = await _context.Yields
                .Where(y => fieldIdList.Contains(y.FieldId))
                .OrderBy(y => y.Season)
                .ToListAsync();

            if (!yields.Any())
            {
                return Ok(new { success = true, message = "Seçilen alanlar için verim kaydı bulunamadı", data = new { } });
            }

            // Alan bazında verim karşılaştırması
            var fieldYields = fieldIdList.Select<int, object>(fieldId =>
            {
                var fieldName = fields.First(f => f.Id == fieldId).Name;
                var fieldYieldData = yields
                    .Where(y => y.FieldId == fieldId)
                    .OrderBy(y => y.Season)
                    .Select(y => new
                    {
                        season = y.Season,
                        amount = y.Amount,
                        unit = y.Unit
                    })
                    .ToList();

                return new
                {
                    fieldId,
                    fieldName,
                    yields = fieldYieldData
                };
            }).ToList();

            // Ortalama verimler
            var averageYields = fieldIdList.Select<int, object>(fieldId =>
            {
                var fieldName = fields.First(f => f.Id == fieldId).Name;
                var fieldYieldData = yields.Where(y => y.FieldId == fieldId);
                
                if (!fieldYieldData.Any())
                    return new { fieldId, fieldName, averageYield = (decimal?)null, unit = "" };
                
                return new
                {
                    fieldId,
                    fieldName,
                    averageYield = fieldYieldData.Average(y => y.Amount),
                    unit = fieldYieldData.First().Unit
                };
            }).ToList();

            return Ok(new
            {
                success = true,
                data = new
                {
                    fieldYields,
                    averageYields
                }
            });
        }

        private bool YieldExists(int id)
        {
            return _context.Yields.Any(e => e.Id == id);
        }
    }
}
