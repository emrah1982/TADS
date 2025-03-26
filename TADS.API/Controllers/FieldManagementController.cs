using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TADS.API.Data;
using TADS.API.Models;

namespace TADS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FieldManagementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FieldManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Field Endpoints

        [HttpGet("fields")]
        [Authorize(Roles = "admin,fieldmanager,superadmin")]
        public async Task<IActionResult> GetFields()
        {
            try
            {
                var fields = await _context.Fields
                    .Include(f => f.SoilType)
                    .Include(f => f.CropType)
                    .Select(f => new
                    {
                        id = f.Id,
                        name = f.Name ?? string.Empty,
                        area = f.Area,
                        location = f.Location ?? string.Empty,
                        latitude = f.Latitude,
                        longitude = f.Longitude,
                        soilType = f.SoilType != null ? f.SoilType.Name : string.Empty,
                        isActive = f.IsActive,
                        cropType = f.CropType != null ? f.CropType.Name : null,
                        plantingDate = f.PlantingDate,
                        harvestDate = f.HarvestDate,
                        notes = f.Notes ?? string.Empty,
                        createdAt = f.CreatedAt,
                        updatedAt = f.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(fields);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error retrieving fields: {ex.Message}" });
            }
        }

        [HttpGet("field/{id}")]
        [Authorize(Roles = "admin,fieldmanager,superadmin")]
        public async Task<IActionResult> GetField(int id)
        {
            try
            {
                var field = await _context.Fields
                    .Include(f => f.SoilType)
                    .Include(f => f.CropType)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (field == null)
                    return NotFound(new { success = false, message = "Field not found" });

                var result = new
                {
                    id = field.Id,
                    name = field.Name,
                    area = field.Area,
                    location = field.Location,
                    latitude = field.Latitude,
                    longitude = field.Longitude,
                    soilTypeId = field.SoilTypeId,
                    soilType = field.SoilType.Name,
                    isActive = field.IsActive,
                    cropTypeId = field.CropTypeId,
                    cropType = field.CropType?.Name,
                    plantingDate = field.PlantingDate,
                    harvestDate = field.HarvestDate,
                    notes = field.Notes,
                    createdAt = field.CreatedAt,
                    createdBy = field.CreatedBy,
                    updatedAt = field.UpdatedAt,
                    updatedBy = field.UpdatedBy
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error retrieving field: {ex.Message}" });
            }
        }

        [HttpPost("field")]
        [Authorize(Roles = "admin,fieldmanager,superadmin")]
        public async Task<IActionResult> CreateField([FromBody] FieldCreateModel model)
        {
            try
            {
                // Gelen veriyi loglama
                var requestJson = System.Text.Json.JsonSerializer.Serialize(model);
                Console.WriteLine($"Gelen veri: {requestJson}");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                    return BadRequest(new { success = false, message = "Invalid field data", errors });
                }

                // Toprak türünü kontrol et
                var soilType = await _context.SoilTypes.FindAsync(model.SoilTypeId);
                if (soilType == null)
                {
                    // Tüm toprak türlerini getir ve hata mesajında göster
                    var allSoilTypes = await _context.SoilTypes.Select(s => new { s.Id, s.Name }).ToListAsync();
                    return BadRequest(new { 
                        success = false, 
                        message = "Invalid soil type", 
                        requestedSoilTypeId = model.SoilTypeId,
                        availableSoilTypes = allSoilTypes 
                    });
                }

                // Ekin türünü kontrol et (eğer belirtilmişse)
                if (model.CropTypeId.HasValue)
                {
                    var cropType = await _context.CropTypes.FindAsync(model.CropTypeId.Value);
                    if (cropType == null)
                    {
                        // Tüm ekin türlerini getir ve hata mesajında göster
                        var allCropTypes = await _context.CropTypes.Select(c => new { c.Id, c.Name }).ToListAsync();
                        return BadRequest(new { 
                            success = false, 
                            message = "Invalid crop type", 
                            requestedCropTypeId = model.CropTypeId.Value,
                            availableCropTypes = allCropTypes 
                        });
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var field = new Field
                {
                    Name = model.Name,
                    Area = model.Area,
                    Location = model.Location,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    SoilTypeId = model.SoilTypeId,
                    CropTypeId = model.CropTypeId,
                    IsActive = model.IsActive,
                    PlantingDate = model.PlantingDate,
                    HarvestDate = model.HarvestDate,
                    Notes = model.Notes,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = userId
                };

                _context.Fields.Add(field);
                
                try {
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Field başarıyla kaydedildi. ID: {field.Id}");
                    return Ok(new { success = true, message = "Field created successfully", fieldId = field.Id });
                }
                catch (Exception ex) {
                    Console.WriteLine($"SaveChangesAsync hatası: {ex.Message}");
                    Console.WriteLine($"İç hata: {ex.InnerException?.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Genel hata: {ex.Message}");
                Console.WriteLine($"İç hata: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "An error occurred while creating the field", 
                    error = ex.Message, 
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        [HttpPut("field/{id}")]
        [Authorize(Roles = "admin,fieldmanager,superadmin")]
        public async Task<IActionResult> UpdateField(int id, [FromBody] FieldUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return BadRequest(new { success = false, message = "Invalid field data", errors });
            }

            try
            {
                var field = await _context.Fields.FindAsync(id);
                if (field == null)
                    return NotFound(new { success = false, message = "Field not found" });

                // Toprak türünü kontrol et
                var soilType = await _context.SoilTypes.FindAsync(model.SoilTypeId);
                if (soilType == null)
                {
                    // Tüm toprak türlerini getir ve hata mesajında göster
                    var allSoilTypes = await _context.SoilTypes.Select(s => new { s.Id, s.Name }).ToListAsync();
                    return BadRequest(new { 
                        success = false, 
                        message = "Invalid soil type", 
                        requestedSoilTypeId = model.SoilTypeId,
                        availableSoilTypes = allSoilTypes 
                    });
                }

                // Ekin türünü kontrol et (eğer belirtilmişse)
                if (model.CropTypeId.HasValue)
                {
                    var cropType = await _context.CropTypes.FindAsync(model.CropTypeId.Value);
                    if (cropType == null)
                    {
                        // Tüm ekin türlerini getir ve hata mesajında göster
                        var allCropTypes = await _context.CropTypes.Select(c => new { c.Id, c.Name }).ToListAsync();
                        return BadRequest(new { 
                            success = false, 
                            message = "Invalid crop type", 
                            requestedCropTypeId = model.CropTypeId.Value,
                            availableCropTypes = allCropTypes 
                        });
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                field.Name = model.Name;
                field.Area = model.Area;
                field.Location = model.Location;
                field.Latitude = model.Latitude;
                field.Longitude = model.Longitude;
                field.SoilTypeId = model.SoilTypeId;
                field.CropTypeId = model.CropTypeId;
                field.IsActive = model.IsActive;
                field.PlantingDate = model.PlantingDate;
                field.HarvestDate = model.HarvestDate;
                field.Notes = model.Notes;
                field.UpdatedAt = DateTime.Now;
                field.UpdatedBy = userId;

                _context.Entry(field).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Parsel geçmişine güncelleme kaydı ekle
                var history = new FieldHistory
                {
                    FieldId = field.Id,
                    ActionType = "Güncelleme",
                    Description = "Parsel bilgileri güncellendi",
                    ActionDate = DateTime.Now,
                    ActionBy = userId,
                    Notes = model.Notes
                };

                _context.FieldHistories.Add(history);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Field updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating the field", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpDelete("field/{id}")]
        [Authorize(Roles = "admin,superadmin")]
        public async Task<IActionResult> DeleteField(int id)
        {
            try
            {
                var field = await _context.Fields.FindAsync(id);
                if (field == null)
                    return NotFound(new { success = false, message = "Field not found" });

                _context.Fields.Remove(field);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Field deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error deleting field: {ex.Message}" });
            }
        }

        #endregion

        #region SoilType Endpoints

        [HttpGet("soil-types")]
        [Authorize(Roles = "admin,fieldmanager,analyst,viewer,superadmin")]
        public async Task<IActionResult> GetSoilTypes()
        {
            try
            {
                var soilTypes = await _context.SoilTypes.ToListAsync();
                return Ok(soilTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Toprak türleri alınırken bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpPost("soil-type")]
        [Authorize(Roles = "admin,superadmin")]
        public async Task<IActionResult> CreateSoilType([FromBody] SoilTypeRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Geçersiz veri formatı" });
            }

            try
            {
                // Aynı isimde toprak türü var mı kontrol et
                var existingSoilType = await _context.SoilTypes.FirstOrDefaultAsync(s => s.Name.ToLower() == model.Name.ToLower());
                if (existingSoilType != null)
                {
                    return BadRequest(new { message = "Bu isimde bir toprak türü zaten mevcut" });
                }

                var soilType = new SoilType
                {
                    Name = model.Name,
                    Description = model.Description
                };

                _context.SoilTypes.Add(soilType);
                await _context.SaveChangesAsync();

                return Ok(soilType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Toprak türü eklenirken bir hata oluştu: {ex.Message}" });
            }
        }

        #endregion

        #region CropType Endpoints

        [HttpGet("crop-types")]
        [Authorize(Roles = "admin,fieldmanager,analyst,viewer,superadmin")]
        public async Task<IActionResult> GetCropTypes()
        {
            try
            {
                var cropTypes = await _context.CropTypes
                    .Select(ct => new
                    {
                        id = ct.Id,
                        name = ct.Name,
                        description = ct.Description,
                        growingSeason = ct.GrowingSeason,
                        growingDays = ct.GrowingDays
                    })
                    .ToListAsync();

                return Ok(cropTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error retrieving crop types: {ex.Message}" });
            }
        }

        [HttpPost("crop-type")]
        [Authorize(Roles = "admin,superadmin")]
        public async Task<IActionResult> CreateCropType([FromBody] CropTypeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid crop type data", errors = ModelState });

            try
            {
                var cropType = new CropType
                {
                    Name = model.Name ?? string.Empty,
                    Description = model.Description ?? string.Empty,
                    GrowingSeason = model.GrowingSeason ?? string.Empty,
                    GrowingDays = model.GrowingDays
                };

                _context.CropTypes.Add(cropType);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCropTypes), new { success = true, message = "Crop type created successfully", cropTypeId = cropType.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error creating crop type: {ex.Message}" });
            }
        }

        #endregion

        #region FieldHistory Endpoints

        [HttpGet("field/{fieldId}/history")]
        public async Task<IActionResult> GetFieldHistory(int fieldId)
        {
            try
            {
                var field = await _context.Fields.FindAsync(fieldId);
                if (field == null)
                    return NotFound(new { success = false, message = "Field not found" });

                var history = await _context.FieldHistories
                    .Where(h => h.FieldId == fieldId)
                    .OrderByDescending(h => h.ActionDate)
                    .Select(h => new
                    {
                        id = h.Id,
                        actionType = h.ActionType,
                        description = h.Description,
                        actionDate = h.ActionDate,
                        actionBy = h.ActionBy,
                        cost = h.Cost,
                        notes = h.Notes
                    })
                    .ToListAsync();

                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error retrieving field history: {ex.Message}" });
            }
        }

        [HttpPost("field/{fieldId}/history")]
        public async Task<IActionResult> AddFieldHistory(int fieldId, [FromBody] FieldHistoryModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid field history data", errors = ModelState });

            try
            {
                var field = await _context.Fields.FindAsync(fieldId);
                if (field == null)
                    return NotFound(new { success = false, message = "Field not found" });

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var history = new FieldHistory
                {
                    FieldId = fieldId,
                    ActionType = model.ActionType,
                    Description = model.Description,
                    ActionDate = model.ActionDate,
                    ActionBy = userId,
                    Cost = model.Cost,
                    Notes = model.Notes
                };

                _context.FieldHistories.Add(history);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetFieldHistory), new { fieldId = fieldId }, new { success = true, message = "Field history added successfully", historyId = history.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error adding field history: {ex.Message}" });
            }
        }

        #endregion

        #region Seed Data

        [HttpPost("seed-data")]
        [AllowAnonymous] // Geliştirme aşamasında kolay erişim için
        public async Task<IActionResult> SeedData()
        {
            try
            {
                // Veritabanında veri var mı kontrol et
                if (_context.SoilTypes.Any() || _context.CropTypes.Any())
                {
                    return BadRequest(new { success = false, message = "Database already contains seed data" });
                }

                // Toprak türleri
                var soilTypes = new List<SoilType>
                {
                    new SoilType { Name = "Kumlu Toprak", Description = "Kumlu topraklar, büyük parçacıklardan oluşur ve su tutma kapasitesi düşüktür." },
                    new SoilType { Name = "Killi Toprak", Description = "Killi topraklar, küçük parçacıklardan oluşur ve su tutma kapasitesi yüksektir." },
                    new SoilType { Name = "Tınlı Toprak", Description = "Tınlı topraklar, kum, kil ve organik maddenin dengeli bir karışımıdır." },
                    new SoilType { Name = "Kireçli Toprak", Description = "Kireçli topraklar, yüksek kalsiyum karbonat içeriğine sahiptir." },
                    new SoilType { Name = "Humuslu Toprak", Description = "Humuslu topraklar, yüksek organik madde içeriğine sahiptir." },
                    new SoilType { Name = "Çakıllı Toprak", Description = "Çakıllı topraklar, büyük taş parçaları içerir ve drenaj kapasitesi yüksektir." },
                    new SoilType { Name = "Balçık", Description = "Balçık, kil ve kumun karışımından oluşan verimli bir toprak türüdür." }
                };

                await _context.SoilTypes.AddRangeAsync(soilTypes);

                // Ekin türleri
                var cropTypes = new List<CropType>
                {
                    new CropType { Name = "Buğday", Description = "Tahıl ürünü", GrowingSeason = "Sonbahar-Yaz", GrowingDays = 240 },
                    new CropType { Name = "Arpa", Description = "Tahıl ürünü", GrowingSeason = "Sonbahar-Yaz", GrowingDays = 210 },
                    new CropType { Name = "Mısır", Description = "Tahıl ürünü", GrowingSeason = "İlkbahar-Sonbahar", GrowingDays = 120 },
                    new CropType { Name = "Pamuk", Description = "Endüstriyel ürün", GrowingSeason = "İlkbahar-Sonbahar", GrowingDays = 160 },
                    new CropType { Name = "Ayçiçeği", Description = "Yağlı tohum", GrowingSeason = "İlkbahar-Sonbahar", GrowingDays = 110 },
                    new CropType { Name = "Şeker Pancarı", Description = "Kök ürün", GrowingSeason = "İlkbahar-Sonbahar", GrowingDays = 180 },
                    new CropType { Name = "Patates", Description = "Yumru ürün", GrowingSeason = "İlkbahar-Sonbahar", GrowingDays = 100 },
                    new CropType { Name = "Domates", Description = "Sebze", GrowingSeason = "İlkbahar-Sonbahar", GrowingDays = 80 },
                    new CropType { Name = "Biber", Description = "Sebze", GrowingSeason = "İlkbahar-Sonbahar", GrowingDays = 70 },
                    new CropType { Name = "Üzüm", Description = "Meyve", GrowingSeason = "Çok yıllık", GrowingDays = null },
                    new CropType { Name = "Zeytin", Description = "Meyve", GrowingSeason = "Çok yıllık", GrowingDays = null },
                    new CropType { Name = "Elma", Description = "Meyve", GrowingSeason = "Çok yıllık", GrowingDays = null },
                    new CropType { Name = "Diğer", Description = "Diğer ürünler", GrowingSeason = "Değişken", GrowingDays = null }
                };

                await _context.CropTypes.AddRangeAsync(cropTypes);
                await _context.SaveChangesAsync();

                // Örnek parseller
                if (!_context.Fields.Any())
                {
                    var fields = new List<Field>
                    {
                        new Field
                        {
                            Name = "Kuzey Tarla",
                            Area = 45.5m,
                            Location = "Ankara, Polatlı",
                            Latitude = 39.5833m,
                            Longitude = 32.1167m,
                            SoilTypeId = soilTypes[1].Id, // Killi Toprak
                            CropTypeId = cropTypes[0].Id, // Buğday
                            IsActive = true,
                            PlantingDate = new DateTime(2024, 10, 15),
                            HarvestDate = new DateTime(2025, 6, 20),
                            Notes = "Sulama sistemi yenilendi",
                            CreatedAt = DateTime.Now,
                            CreatedBy = "system"
                        },
                        new Field
                        {
                            Name = "Güney Bahçe",
                            Area = 12.8m,
                            Location = "Ankara, Polatlı",
                            Latitude = 39.5667m,
                            Longitude = 32.1333m,
                            SoilTypeId = soilTypes[4].Id, // Humuslu Toprak
                            CropTypeId = cropTypes[1].Id, // Arpa
                            IsActive = true,
                            PlantingDate = new DateTime(2024, 4, 10),
                            HarvestDate = new DateTime(2024, 8, 15),
                            Notes = "Organik tarım yapılıyor",
                            CreatedAt = DateTime.Now,
                            CreatedBy = "system"
                        },
                        new Field
                        {
                            Name = "Batı Parsel",
                            Area = 28.3m,
                            Location = "Ankara, Haymana",
                            Latitude = 39.4333m,
                            Longitude = 32.6333m,
                            SoilTypeId = soilTypes[2].Id, // Tınlı Toprak
                            CropTypeId = cropTypes[2].Id, // Mısır
                            IsActive = false,
                            PlantingDate = new DateTime(2024, 5, 1),
                            HarvestDate = new DateTime(2024, 9, 30),
                            Notes = "Nadasa bırakıldı",
                            CreatedAt = DateTime.Now,
                            CreatedBy = "system"
                        },
                        new Field
                        {
                            Name = "Doğu Bağ",
                            Area = 8.2m,
                            Location = "Ankara, Beypazarı",
                            Latitude = 40.1667m,
                            Longitude = 31.9167m,
                            SoilTypeId = soilTypes[3].Id, // Kireçli Toprak
                            CropTypeId = cropTypes[3].Id, // Pamuk
                            IsActive = true,
                            PlantingDate = new DateTime(2023, 3, 15),
                            HarvestDate = new DateTime(2024, 9, 10),
                            Notes = "Yeni asma dikimi yapıldı",
                            CreatedAt = DateTime.Now,
                            CreatedBy = "system"
                        }
                    };

                    await _context.Fields.AddRangeAsync(fields);
                    await _context.SaveChangesAsync();

                    // Örnek parsel geçmişi
                    var histories = new List<FieldHistory>();
                    foreach (var field in fields)
                    {
                        histories.Add(new FieldHistory
                        {
                            FieldId = field.Id,
                            ActionType = "Oluşturma",
                            Description = "Parsel kaydı oluşturuldu",
                            ActionDate = DateTime.Now.AddDays(-30),
                            ActionBy = "system",
                            Notes = "İlk kayıt"
                        });

                        histories.Add(new FieldHistory
                        {
                            FieldId = field.Id,
                            ActionType = "Ekim",
                            Description = "Ekim yapıldı",
                            ActionDate = field.PlantingDate ?? DateTime.Now.AddDays(-20),
                            ActionBy = "system",
                            Cost = 1500.00m,
                            Notes = "Tohum ve gübre maliyeti dahil"
                        });
                    }

                    await _context.FieldHistories.AddRangeAsync(histories);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { success = true, message = "Seed data created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error creating seed data: {ex.Message}" });
            }
        }

        #endregion
    }

    #region Request Models

    public class FieldCreateModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Range(0.1, 10000)]
        public decimal Area { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;
        
        public decimal? Latitude { get; set; }
        
        public decimal? Longitude { get; set; }
        
        [Required]
        public int SoilTypeId { get; set; }
        
        public int? CropTypeId { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime? PlantingDate { get; set; }
        
        public DateTime? HarvestDate { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class FieldUpdateModel : FieldCreateModel
    {
        [Required]
        public int Id { get; set; }
    }

    public class SoilTypeRequest
    {
        [Required(ErrorMessage = "Toprak türü adı zorunludur")]
        [StringLength(50, ErrorMessage = "Toprak türü adı en fazla 50 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur")]
        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string Description { get; set; } = string.Empty;
    }

    public class CropTypeModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        [StringLength(50)]
        public string? GrowingSeason { get; set; }
        
        public int? GrowingDays { get; set; }
    }

    public class FieldHistoryModel
    {
        [Required]
        [StringLength(50)]
        public string ActionType { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime ActionDate { get; set; }
        
        public decimal? Cost { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    #endregion
}
