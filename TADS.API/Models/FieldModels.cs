using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TADS.API.Models
{
    /// <summary>
    /// Tarım alanı/parsel bilgilerini temsil eder
    /// </summary>
    public class Field
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Area { get; set; } // Dönüm cinsinden alan
        
        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;
        
        // GPS koordinatları
        [Column(TypeName = "decimal(10, 6)")]
        public decimal? Latitude { get; set; }
        
        [Column(TypeName = "decimal(10, 6)")]
        public decimal? Longitude { get; set; }
        
        [Required]
        public int SoilTypeId { get; set; }
        
        [ForeignKey("SoilTypeId")]
        public virtual SoilType? SoilType { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int? CropTypeId { get; set; }
        
        [ForeignKey("CropTypeId")]
        public virtual CropType? CropType { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime? PlantingDate { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime? HarvestDate { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        // Oluşturma ve güncelleme bilgileri
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [StringLength(450)]
        public string? CreatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        [StringLength(450)]
        public string? UpdatedBy { get; set; }
        
        // İlişkiler
        public virtual ICollection<FieldHistory> FieldHistories { get; set; } = new List<FieldHistory>();
        public virtual ICollection<FieldSensorData> SensorData { get; set; } = new List<FieldSensorData>();
        public virtual ICollection<FieldImage> Images { get; set; } = new List<FieldImage>();
        public virtual ICollection<FieldCropRotation> CropRotations { get; set; } = new List<FieldCropRotation>();
        public virtual ICollection<Irrigation> Irrigations { get; set; } = new List<Irrigation>();
        public virtual ICollection<Fertilization> Fertilizations { get; set; } = new List<Fertilization>();
        public virtual ICollection<Yield> Yields { get; set; } = new List<Yield>();
    }
    
    /// <summary>
    /// Toprak türlerini temsil eder
    /// </summary>
    public class SoilType
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        // İlişkiler
        [JsonIgnore]
        public virtual ICollection<Field> Fields { get; set; } = new List<Field>();
    }
    
    /// <summary>
    /// Ekin/ürün türlerini temsil eder
    /// </summary>
    public class CropType
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string GrowingSeason { get; set; } = string.Empty;
        
        [Column(TypeName = "int")]
        public int? GrowingDays { get; set; }
        
        // İlişkiler
        [JsonIgnore]
        public virtual ICollection<Field> Fields { get; set; } = new List<Field>();
        [JsonIgnore]
        public virtual ICollection<FieldCropRotation> CropRotations { get; set; } = new List<FieldCropRotation>();
    }
    
    /// <summary>
    /// Parsel geçmişini temsil eder (değişiklikler, işlemler)
    /// </summary>
    public class FieldHistory
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FieldId { get; set; }
        
        [ForeignKey("FieldId")]
        [JsonIgnore]
        public virtual Field? Field { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ActionType { get; set; } = string.Empty; // Örn: "Ekim", "Gübreleme", "İlaçlama", "Sulama", "Hasat"
        
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime ActionDate { get; set; } = DateTime.Now;
        
        [StringLength(450)]
        public string ActionBy { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Cost { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Parsel sensör verilerini temsil eder
    /// </summary>
    public class FieldSensorData
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FieldId { get; set; }
        
        [ForeignKey("FieldId")]
        [JsonIgnore]
        public virtual Field? Field { get; set; }
        
        [Required]
        [StringLength(50)]
        public string SensorType { get; set; } = string.Empty; // Örn: "Sıcaklık", "Nem", "pH", "Yağış"
        
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Value { get; set; }
        
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty; // Örn: "°C", "%", "mm"
        
        [Required]
        public DateTime ReadingTime { get; set; } = DateTime.Now;
        
        [StringLength(100)]
        public string SensorId { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Location { get; set; } = string.Empty; // Sensörün parseldeki konumu
    }
    
    /// <summary>
    /// Parsel görüntülerini temsil eder
    /// </summary>
    public class FieldImage
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FieldId { get; set; }
        
        [ForeignKey("FieldId")]
        [JsonIgnore]
        public virtual Field? Field { get; set; }
        
        [Required]
        [StringLength(255)]
        public string ImagePath { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime UploadDate { get; set; } = DateTime.Now;
        
        [StringLength(50)]
        public string ImageType { get; set; } = string.Empty; // Örn: "Uydu", "Drone", "Normal"
        
        public bool IsActive { get; set; } = true;
    }
    
    /// <summary>
    /// Ekim nöbeti (münavebe) planını temsil eder
    /// </summary>
    public class FieldCropRotation
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FieldId { get; set; }
        
        [ForeignKey("FieldId")]
        [JsonIgnore]
        public virtual Field? Field { get; set; }
        
        [Required]
        public int CropTypeId { get; set; }
        
        [ForeignKey("CropTypeId")]
        public virtual CropType? CropType { get; set; }
        
        [Required]
        public int RotationOrder { get; set; }
        
        [Required]
        public int Year { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime? PlannedPlantingDate { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime? PlannedHarvestDate { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        public bool IsCompleted { get; set; } = false;
    }
}
