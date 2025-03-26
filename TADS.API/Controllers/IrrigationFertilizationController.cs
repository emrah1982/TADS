using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TADS.API.Data;
using TADS.API.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TADS.API.Controllers
{
    [Route("api/irrigation-fertilization")]
    [ApiController]
    public class IrrigationFertilizationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public IrrigationFertilizationController(ApplicationDbContext context, ILogger<IrrigationFertilizationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Irrigation Methods

        // GET: api/irrigation-fertilization/irrigation-list
        [HttpGet("irrigation-list")]
        public async Task<IActionResult> GetIrrigations()
        {
            var irrigations = await _context.Irrigations
                .Include(i => i.Field)
                .OrderByDescending(i => i.Date)
                .ToListAsync();

            return Ok(irrigations);
        }

        // GET: api/irrigation-fertilization/irrigation/field/{fieldId}
        [HttpGet("irrigation/field/{fieldId}")]
        public async Task<IActionResult> GetIrrigationsByField(int fieldId)
        {
            try
            {
                _logger.LogInformation($"GetIrrigationsByField çağrıldı. FieldId: {fieldId}");
                Console.WriteLine($"GetIrrigationsByField çağrıldı. FieldId: {fieldId}");

                // Alan kontrolü
                var field = await _context.Fields.FirstOrDefaultAsync(f => f.Id == fieldId);

                if (field == null)
                {
                    _logger.LogWarning($"Alan bulunamadı. FieldId: {fieldId}");
                    Console.WriteLine($"Alan bulunamadı. FieldId: {fieldId}");
                    
                    // Mevcut alanların listesini döndürelim
                    var fields = await _context.Fields
                        .Select(f => new { f.Id, f.Name })
                        .ToListAsync();

                    return NotFound(new { 
                        success = false, 
                        message = "Alan bulunamadı", 
                        availableFields = fields 
                    });
                }

                var irrigations = await _context.Irrigations
                    .Where(i => i.FieldId == fieldId)
                    .OrderByDescending(i => i.Date)
                    .ToListAsync();

                _logger.LogInformation($"Bulunan sulama kayıt sayısı: {irrigations.Count}");
                Console.WriteLine($"Bulunan sulama kayıt sayısı: {irrigations.Count}");
                
                return Ok(irrigations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetIrrigationsByField hatası. FieldId: {fieldId}. Hata: {ex.Message}");
                Console.WriteLine($"GetIrrigationsByField hatası. FieldId: {fieldId}. Hata: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                return StatusCode(500, new
                {
                    success = false,
                    message = "Sulama kayıtları getirilirken bir hata oluştu",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        // GET: api/irrigation-fertilization/irrigation-get/{id}
        [HttpGet("irrigation-get/{id}")]
        public async Task<IActionResult> GetIrrigation(int id)
        {
            var irrigation = await _context.Irrigations
                .Include(i => i.Field)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (irrigation == null)
                return NotFound(new { success = false, message = "Sulama kaydı bulunamadı" });

            return Ok(irrigation);
        }

        // GET: api/irrigation-fertilization/field/{fieldId}/irrigations
        [HttpGet("field/{fieldId}/irrigations")]
        public async Task<IActionResult> GetIrrigationsByFieldId(int fieldId)
        {
            try
            {
                // Alan kontrolü
                var field = await _context.Fields.FindAsync(fieldId);
                if (field == null)
                {
                    return NotFound(new { success = false, message = $"Alan bulunamadı. ID: {fieldId}" });
                }

                var irrigations = await _context.Irrigations
                    .Where(i => i.FieldId == fieldId)
                    .OrderByDescending(i => i.Date)
                    .ToListAsync();

                return Ok(new { success = true, data = irrigations });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Sulama kayıtları getirilirken hata: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Sulama kayıtları getirilirken bir hata oluştu", error = ex.Message });
            }
        }

        // GET: api/irrigation-fertilization/irrigation-detail/{id}
        [HttpGet("irrigation-detail/{id}")]
        public async Task<IActionResult> GetIrrigationById(int id)
        {
            try
            {
                var irrigation = await _context.Irrigations
                    .Include(i => i.Field)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (irrigation == null)
                {
                    return NotFound(new { success = false, message = $"Sulama kaydı bulunamadı. ID: {id}" });
                }

                return Ok(new { success = true, data = irrigation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Sulama kaydı getirilirken hata: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Sulama kaydı getirilirken bir hata oluştu", error = ex.Message });
            }
        }

        // POST: api/irrigation-fertilization/irrigation
        [HttpPost("irrigation")]
        public async Task<IActionResult> CreateIrrigation([FromBody] Irrigation irrigationData)
        {
            try
            {
                // Model doğrulama
                if (irrigationData == null)
                {
                    _logger.LogError("Gelen veri null");
                    return BadRequest(new { success = false, message = "Geçersiz veri formatı: Boş veri" });
                }

                _logger.LogInformation($"Gelen sulama verisi: FieldId={irrigationData.FieldId}, Date={irrigationData.Date}, Amount={irrigationData.Amount}");
                Console.WriteLine($"Gelen sulama verisi: FieldId={irrigationData.FieldId}, Date={irrigationData.Date}, Amount={irrigationData.Amount}");

                // FieldId kontrolü
                if (irrigationData.FieldId <= 0)
                {
                    _logger.LogError($"Geçersiz FieldId: {irrigationData.FieldId}");
                    Console.WriteLine($"Geçersiz FieldId: {irrigationData.FieldId}");
                    
                    // Mevcut alanların listesini döndürelim
                    var fields = await _context.Fields.Select(f => new { f.Id, f.Name }).ToListAsync();
                    return BadRequest(new { 
                        success = false, 
                        message = "Geçersiz alan ID'si", 
                        availableFields = fields 
                    });
                }

                // Alan kontrolü
                var field = await _context.Fields.FindAsync(irrigationData.FieldId);
                if (field == null)
                {
                    _logger.LogError($"Alan bulunamadı: {irrigationData.FieldId}");
                    Console.WriteLine($"Alan bulunamadı: {irrigationData.FieldId}");
                    
                    // Mevcut alanların listesini döndürelim
                    var fields = await _context.Fields.Select(f => new { f.Id, f.Name }).ToListAsync();
                    return BadRequest(new { 
                        success = false, 
                        message = $"Alan bulunamadı: ID={irrigationData.FieldId}", 
                        availableFields = fields 
                    });
                }

                // Model oluşturma
                var irrigation = new Irrigation
                {
                    FieldId = irrigationData.FieldId,
                    Date = irrigationData.Date,
                    Amount = irrigationData.Amount,
                    Unit = irrigationData.Unit,
                    Method = irrigationData.Method,
                    Notes = irrigationData.Notes,
                    SoilMoistureBeforeIrrigation = irrigationData.SoilMoistureBeforeIrrigation,
                    SoilMoistureAfterIrrigation = irrigationData.SoilMoistureAfterIrrigation,
                    Season = irrigationData.Season,
                    CreatedAt = DateTime.Now,
                    CreatedBy = User.Identity?.Name ?? "anonymous"
                };

                _context.Irrigations.Add(irrigation);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = irrigation, message = "Sulama kaydı başarıyla oluşturuldu" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Sulama kaydı oluşturma hatası: {ex.Message}");
                Console.WriteLine($"Sulama kaydı oluşturma hatası: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Sulama kaydı oluşturulurken bir hata oluştu", error = ex.Message });
            }
        }

        // PUT: api/irrigation-fertilization/irrigation/{id}
        [HttpPut("irrigation/{id}")]
        public async Task<IActionResult> UpdateIrrigation(int id, [FromBody] Irrigation irrigation)
        {
            if (id != irrigation.Id)
                return BadRequest(new { success = false, message = "ID uyuşmazlığı" });

            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz model", errors = ModelState });

            var existingIrrigation = await _context.Irrigations.FindAsync(id);
            if (existingIrrigation == null)
                return NotFound(new { success = false, message = "Sulama kaydı bulunamadı" });

            existingIrrigation.Date = irrigation.Date;
            existingIrrigation.Amount = irrigation.Amount;
            existingIrrigation.Unit = irrigation.Unit ?? "litre";
            existingIrrigation.Method = irrigation.Method ?? "Damla";
            existingIrrigation.Notes = irrigation.Notes ?? string.Empty;
            existingIrrigation.SoilMoistureBeforeIrrigation = irrigation.SoilMoistureBeforeIrrigation;
            existingIrrigation.SoilMoistureAfterIrrigation = irrigation.SoilMoistureAfterIrrigation;
            existingIrrigation.UpdatedAt = DateTime.Now;
            existingIrrigation.UpdatedBy = User.Identity?.Name ?? "system";

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Sulama kaydı güncellendi", data = existingIrrigation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Sulama kaydı güncellenirken bir hata oluştu", error = ex.Message });
            }
        }

        // DELETE: api/irrigation-fertilization/irrigation/{id}
        [HttpDelete("irrigation/{id}")]
        public async Task<IActionResult> DeleteIrrigation(int id)
        {
            var irrigation = await _context.Irrigations.FindAsync(id);
            if (irrigation == null)
                return NotFound(new { success = false, message = "Sulama kaydı bulunamadı" });

            _context.Irrigations.Remove(irrigation);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Sulama kaydı silindi" });
        }

        private bool IrrigationExists(int id)
        {
            return _context.Irrigations.Any(e => e.Id == id);
        }

        #endregion

        #region Fertilization Methods

        // GET: api/irrigation-fertilization/fertilization-list
        [HttpGet("fertilization-list")]
        public async Task<IActionResult> GetFertilizations()
        {
            var fertilizations = await _context.Fertilizations
                .Include(f => f.Field)
                .Include(f => f.FertilizerType)
                .OrderByDescending(f => f.Date)
                .ToListAsync();

            return Ok(fertilizations);
        }

        // GET: api/irrigation-fertilization/fertilization/field/{fieldId}
        [HttpGet("fertilization/field/{fieldId}")]
        public async Task<IActionResult> GetFertilizationsByField(int fieldId)
        {
            var field = await _context.Fields.FindAsync(fieldId);
            if (field == null)
                return NotFound(new { success = false, message = "Alan bulunamadı" });

            var fertilizations = await _context.Fertilizations
                .Include(f => f.FertilizerType)
                .Where(f => f.FieldId == fieldId)
                .OrderByDescending(f => f.Date)
                .ToListAsync();

            return Ok(fertilizations);
        }

        // GET: api/irrigation-fertilization/fertilization-get/{id}
        [HttpGet("fertilization-get/{id}")]
        public async Task<IActionResult> GetFertilization(int id)
        {
            var fertilization = await _context.Fertilizations
                .Include(f => f.Field)
                .Include(f => f.FertilizerType)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fertilization == null)
                return NotFound(new { success = false, message = "Gübreleme kaydı bulunamadı" });

            return Ok(fertilization);
        }

        // GET: api/irrigation-fertilization/field/{fieldId}/fertilizations
        [HttpGet("field/{fieldId}/fertilizations")]
        public async Task<IActionResult> GetFertilizationsByFieldId(int fieldId)
        {
            try
            {
                // Alan kontrolü
                var field = await _context.Fields.FindAsync(fieldId);
                if (field == null)
                {
                    return NotFound(new { success = false, message = $"Alan bulunamadı. ID: {fieldId}" });
                }

                var fertilizations = await _context.Fertilizations
                    .Include(f => f.FertilizerType)
                    .Where(f => f.FieldId == fieldId)
                    .OrderByDescending(f => f.Date)
                    .ToListAsync();

                return Ok(new { success = true, data = fertilizations });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Gübreleme kayıtları getirilirken hata: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Gübreleme kayıtları getirilirken bir hata oluştu", error = ex.Message });
            }
        }

        // GET: api/irrigation-fertilization/fertilization-detail/{id}
        [HttpGet("fertilization-detail/{id}")]
        public async Task<IActionResult> GetFertilizationById(int id)
        {
            try
            {
                var fertilization = await _context.Fertilizations
                    .Include(f => f.Field)
                    .Include(f => f.FertilizerType)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (fertilization == null)
                {
                    return NotFound(new { success = false, message = $"Gübreleme kaydı bulunamadı. ID: {id}" });
                }

                return Ok(new { success = true, data = fertilization });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Gübreleme kaydı getirilirken hata: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Gübreleme kaydı getirilirken bir hata oluştu", error = ex.Message });
            }
        }

        // POST: api/irrigation-fertilization/fertilization
        [HttpPost("fertilization")]
        public async Task<IActionResult> CreateFertilization([FromBody] JsonDocument jsonDoc)
        {
            try
            {
                _logger.LogInformation($"Gelen gübreleme verisi (JSON): {jsonDoc}");
                Console.WriteLine($"Gelen gübreleme verisi (JSON): {jsonDoc}");

                // JSON verilerini manuel olarak çıkaralım
                var fertilizationData = new Fertilization();
                
                try
                {
                    using (JsonDocument document = jsonDoc)
                    {
                        JsonElement root = document.RootElement;
                        
                        if (root.TryGetProperty("fieldId", out JsonElement fieldIdElement))
                        {
                            fertilizationData.FieldId = fieldIdElement.GetInt32();
                            _logger.LogInformation($"FieldId: {fertilizationData.FieldId}");
                        }
                        
                        if (root.TryGetProperty("date", out JsonElement dateElement))
                        {
                            fertilizationData.Date = dateElement.GetDateTime();
                            _logger.LogInformation($"Date: {fertilizationData.Date}");
                        }
                        
                        if (root.TryGetProperty("amount", out JsonElement amountElement))
                        {
                            fertilizationData.Amount = amountElement.GetDecimal();
                            _logger.LogInformation($"Amount: {fertilizationData.Amount}");
                        }
                        
                        if (root.TryGetProperty("fertilizerTypeId", out JsonElement fertilizerTypeIdElement))
                        {
                            fertilizationData.FertilizerTypeId = fertilizerTypeIdElement.GetInt32();
                            _logger.LogInformation($"FertilizerTypeId: {fertilizationData.FertilizerTypeId}");
                        }
                        
                        if (root.TryGetProperty("unit", out JsonElement unitElement))
                        {
                            fertilizationData.Unit = unitElement.GetString() ?? "kg";
                            _logger.LogInformation($"Unit: {fertilizationData.Unit}");
                        }
                        
                        if (root.TryGetProperty("method", out JsonElement methodElement))
                        {
                            fertilizationData.Method = methodElement.GetString() ?? "Manuel";
                            _logger.LogInformation($"Method: {fertilizationData.Method}");
                        }
                        
                        if (root.TryGetProperty("notes", out JsonElement notesElement))
                        {
                            fertilizationData.Notes = notesElement.GetString() ?? "";
                            _logger.LogInformation($"Notes: {fertilizationData.Notes}");
                        }
                        
                        if (root.TryGetProperty("season", out JsonElement seasonElement))
                        {
                            fertilizationData.Season = seasonElement.GetInt32();
                            _logger.LogInformation($"Season: {fertilizationData.Season}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"JSON verilerini çıkarırken hata: {ex.Message}");
                    Console.WriteLine($"JSON verilerini çıkarırken hata: {ex.Message}");
                    return BadRequest(new { success = false, message = "Geçersiz veri formatı", error = ex.Message });
                }

                _logger.LogInformation($"Çıkarılan gübreleme verisi: FieldId={fertilizationData.FieldId}, Date={fertilizationData.Date}, Amount={fertilizationData.Amount}, FertilizerTypeId={fertilizationData.FertilizerTypeId}");
                Console.WriteLine($"Çıkarılan gübreleme verisi: FieldId={fertilizationData.FieldId}, Date={fertilizationData.Date}, Amount={fertilizationData.Amount}, FertilizerTypeId={fertilizationData.FertilizerTypeId}");

                // FieldId kontrolü
                if (fertilizationData.FieldId <= 0)
                {
                    _logger.LogError($"Geçersiz FieldId: {fertilizationData.FieldId}");
                    Console.WriteLine($"Geçersiz FieldId: {fertilizationData.FieldId}");
                    
                    // Mevcut alanların listesini döndürelim
                    var fields = await _context.Fields.Select(f => new { f.Id, f.Name }).ToListAsync();
                    return BadRequest(new { 
                        success = false, 
                        message = "Geçersiz alan ID'si", 
                        availableFields = fields 
                    });
                }

                // Alan kontrolü
                var field = await _context.Fields.FindAsync(fertilizationData.FieldId);
                if (field == null)
                {
                    _logger.LogError($"Alan bulunamadı: {fertilizationData.FieldId}");
                    Console.WriteLine($"Alan bulunamadı: {fertilizationData.FieldId}");
                    
                    // Mevcut alanların listesini döndürelim
                    var fields = await _context.Fields.Select(f => new { f.Id, f.Name }).ToListAsync();
                    return BadRequest(new { 
                        success = false, 
                        message = $"Alan bulunamadı: ID={fertilizationData.FieldId}", 
                        availableFields = fields 
                    });
                }

                // FertilizerTypeId kontrolü
                if (fertilizationData.FertilizerTypeId <= 0)
                {
                    // Varsayılan gübre türü ID'si 1 olarak ayarlayalım
                    fertilizationData.FertilizerTypeId = 1;
                    _logger.LogWarning("Gübre türü ID'si belirtilmemiş, varsayılan değer atandı: 1");
                    Console.WriteLine("Gübre türü ID'si belirtilmemiş, varsayılan değer atandı: 1");
                }

                // Gübre türü kontrolü
                var fertilizerType = await _context.FertilizerTypes.FindAsync(fertilizationData.FertilizerTypeId);
                if (fertilizerType == null)
                {
                    _logger.LogError($"Gübre türü bulunamadı: {fertilizationData.FertilizerTypeId}");
                    Console.WriteLine($"Gübre türü bulunamadı: {fertilizationData.FertilizerTypeId}");
                    
                    // Mevcut gübre türlerinin listesini döndürelim
                    var fertilizerTypes = await _context.FertilizerTypes.Select(ft => new { ft.Id, ft.Name, ft.NPK }).ToListAsync();
                    return BadRequest(new { 
                        success = false, 
                        message = $"Gübre türü bulunamadı: ID={fertilizationData.FertilizerTypeId}", 
                        availableFertilizerTypes = fertilizerTypes 
                    });
                }

                // Varsayılan değerleri atayalım
                if (string.IsNullOrEmpty(fertilizationData.Unit))
                    fertilizationData.Unit = "kg";
                
                if (string.IsNullOrEmpty(fertilizationData.Method))
                    fertilizationData.Method = "Manuel";
                
                if (string.IsNullOrEmpty(fertilizationData.Notes))
                    fertilizationData.Notes = "";

                // Oluşturma bilgilerini ekleyelim
                fertilizationData.CreatedAt = DateTime.Now;
                fertilizationData.CreatedBy = User.Identity?.Name ?? "anonymous";

                _context.Fertilizations.Add(fertilizationData);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = fertilizationData, message = "Gübreleme kaydı başarıyla oluşturuldu" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Gübreleme kaydı oluşturma hatası: {ex.Message}");
                Console.WriteLine($"Gübreleme kaydı oluşturma hatası: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Gübreleme kaydı oluşturulurken bir hata oluştu", error = ex.Message });
            }
        }

        // PUT: api/irrigation-fertilization/fertilization/{id}
        [HttpPut("fertilization/{id}")]
        public async Task<IActionResult> UpdateFertilization(int id, [FromBody] Fertilization fertilization)
        {
            if (id != fertilization.Id)
                return BadRequest(new { success = false, message = "ID uyuşmazlığı" });

            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Geçersiz model", errors = ModelState });

            var existingFertilization = await _context.Fertilizations.FindAsync(id);
            if (existingFertilization == null)
                return NotFound(new { success = false, message = "Gübreleme kaydı bulunamadı" });

            existingFertilization.Date = fertilization.Date;
            existingFertilization.FertilizerTypeId = fertilization.FertilizerTypeId;
            existingFertilization.Amount = fertilization.Amount;
            existingFertilization.Unit = fertilization.Unit ?? "kg";
            existingFertilization.Method = fertilization.Method ?? "Manuel";
            existingFertilization.Notes = fertilization.Notes ?? string.Empty;
            existingFertilization.UpdatedAt = DateTime.Now;
            existingFertilization.UpdatedBy = User.Identity?.Name ?? "system";

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Gübreleme kaydı güncellendi", data = existingFertilization });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Gübreleme kaydı güncellenirken bir hata oluştu", error = ex.Message });
            }
        }

        // DELETE: api/irrigation-fertilization/fertilization/{id}
        [HttpDelete("fertilization/{id}")]
        public async Task<IActionResult> DeleteFertilization(int id)
        {
            var fertilization = await _context.Fertilizations.FindAsync(id);
            if (fertilization == null)
                return NotFound(new { success = false, message = "Gübreleme kaydı bulunamadı" });

            _context.Fertilizations.Remove(fertilization);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Gübreleme kaydı silindi" });
        }

        private bool FertilizationExists(int id)
        {
            return _context.Fertilizations.Any(e => e.Id == id);
        }

        // GET: api/irrigation-fertilization/fertilizertypes
        [HttpGet("fertilizertypes")]
        public async Task<IActionResult> GetFertilizerTypes()
        {
            try
            {
                var fertilizerTypes = await _context.FertilizerTypes.ToListAsync();
                return Ok(fertilizerTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Gübre türleri getirilirken hata: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Gübre türleri getirilirken bir hata oluştu", error = ex.Message });
            }
        }

        #endregion
    }
}
